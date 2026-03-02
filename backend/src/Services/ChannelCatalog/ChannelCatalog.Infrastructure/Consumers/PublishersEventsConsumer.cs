using System.Text;
using System.Text.Json;
using ChannelCatalog.Infrastructure.Consumers.Publishers;
using ChannelCatalog.Infrastructure.Inbox;
using ChannelCatalog.Infrastructure.Persistence;
using Marketplace.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChannelCatalog.Infrastructure.Consumers;

public sealed class PublishersEventsConsumer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<PublishersEventsConsumer> _log;
    private readonly RabbitMqOptions _opt;

    private IConnection? _conn;
    private IModel? _ch;
    private CancellationToken _stoppingToken;

    public PublishersEventsConsumer(
        IServiceProvider sp,
        IOptions<RabbitMqOptions> opt,
        ILogger<PublishersEventsConsumer> log)
    {
        _sp = sp;
        _log = log;
        _opt = opt.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        var factory = new ConnectionFactory
        {
            HostName = _opt.Host,
            Port = _opt.Port,
            UserName = _opt.Username,
            Password = _opt.Password,
            DispatchConsumersAsync = true
        };

        _conn = factory.CreateConnection();
        _ch = _conn.CreateModel();

        _ch.ExchangeDeclare("marketplace.events", type: ExchangeType.Topic, durable: true, autoDelete: false);

        EnsureQueues();

        const string queue = "catalog.events";

        _ch.QueueBind(queue, "marketplace.events", "publishers.channel.registered");
        _ch.QueueBind(queue, "marketplace.events", "publishers.channel.ownership_verified");

        _ch.BasicQos(0, prefetchCount: 10, global: false);

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.Received += HandleAsync;

        _ch.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

        _log.LogInformation("PublishersEventsConsumer started. Queue={Queue}", queue);
        return Task.CompletedTask;
    }

    private async Task HandleAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var payload = Encoding.UTF8.GetString(ea.Body.ToArray());
            var routingKey = ea.RoutingKey;

            var messageId = ea.BasicProperties?.MessageId;
            if (string.IsNullOrWhiteSpace(messageId))
                throw new InvalidOperationException("MessageId is missing (publisher must set it).");

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler>();
            var handler = handlers.FirstOrDefault(h => h.RoutingKey == routingKey)
                         ?? throw new InvalidOperationException($"No handler for routing key: {routingKey}");

            await using var tx = await db.Database.BeginTransactionAsync(_stoppingToken);

            var alreadyProcessed = await db.InboxMessages.AnyAsync(x => x.MessageId == messageId, _stoppingToken);
            if (alreadyProcessed)
            {
                await tx.CommitAsync(_stoppingToken);
                _ch!.BasicAck(ea.DeliveryTag, multiple: false);
                return;
            }

            await handler.HandleAsync(payload, _stoppingToken);

            db.InboxMessages.Add(new InboxMessage
            {
                MessageId = messageId,
                RoutingKey = routingKey,
                ProcessedAt = DateTimeOffset.UtcNow
            });

            await db.SaveChangesAsync(_stoppingToken);
            await tx.CommitAsync(_stoppingToken);

            _ch!.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (JsonException ex)
        {
            _log.LogError(ex, "Poison message (JSON). routingKey={Key}", ea.RoutingKey);
            _ch!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("No handler"))
        {
            _log.LogError(ex, "No handler for routingKey={Key}", ea.RoutingKey);
            _ch!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Transient error. routingKey={Key}", ea.RoutingKey);
            _ch!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
        }
    }

    public override void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
        base.Dispose();
    }

    private void EnsureQueues()
    {
        const string queue = "catalog.events";
        const string dlx = "catalog.dlx";
        const string dlq = "catalog.events.dlq";

        _ch.ExchangeDeclare(dlx, ExchangeType.Fanout, durable: true, autoDelete: false);
        _ch.QueueDeclare(dlq, durable: true, exclusive: false, autoDelete: false);
        _ch.QueueBind(dlq, dlx, routingKey: string.Empty);

        var queueArgs = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = dlx
        };

        try
        {
            _ch.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            // Channel is closed after precondition failure; recreate and declare cleanly.
            _ch?.Dispose();
            _ch = _conn!.CreateModel();

            _ch.ExchangeDeclare(dlx, ExchangeType.Fanout, durable: true, autoDelete: false);
            _ch.QueueDeclare(dlq, durable: true, exclusive: false, autoDelete: false);
            _ch.QueueBind(dlq, dlx, routingKey: string.Empty);

            _ch.QueueDelete(queue, ifUnused: false, ifEmpty: false);
            _ch.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
        }
    }
}
