using System.Text;
using Marketplace.Messaging.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Deals.Infrastructure.Consumers;

public sealed class PaymentsEventsConsumer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<PaymentsEventsConsumer> _log;
    private readonly RabbitMqOptions _opt;

    private IConnection? _conn;
    private IModel? _ch;
    private CancellationToken _stoppingToken;

    public PaymentsEventsConsumer(IServiceProvider sp, IOptions<RabbitMqOptions> opt, ILogger<PaymentsEventsConsumer> log)
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

        var queue = "deals.events";
        var args = new Dictionary<string, object>();
        _ch.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: args);

        _ch.QueueBind(queue, "marketplace.events", "payments.payment.authorized.v1");
        _ch.QueueBind(queue, "marketplace.events", "payments.payment.captured.v1");
        _ch.QueueBind(queue, "marketplace.events", "payments.payment.canceled.v1");

        _ch.BasicQos(0, prefetchCount: 10, global: false);

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.Received += HandleAsync;
        _ch.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

        _log.LogInformation("PaymentsEventsConsumer started. Queue={Queue}", queue);
        return Task.CompletedTask;
    }

    private async Task HandleAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var payload = Encoding.UTF8.GetString(ea.Body.ToArray());
            var routingKey = ea.RoutingKey;

            using var scope = _sp.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler>();
            var handler = handlers.FirstOrDefault(h => h.RoutingKey == routingKey);
            if (handler is null)
            {
                _log.LogWarning("No handler for routing key {Key}", routingKey);
                _ch!.BasicAck(ea.DeliveryTag, false);
                return;
            }

            await handler.HandleAsync(payload, _stoppingToken);

            _ch!.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to process payments event {Key}", ea.RoutingKey);
            _ch!.BasicNack(ea.DeliveryTag, false, requeue: true);
        }
    }

    public override void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
        base.Dispose();
    }
}
