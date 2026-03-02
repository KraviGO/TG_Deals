using System.Text;
using RabbitMQ.Client;

namespace Marketplace.Messaging.RabbitMq;

public sealed class RabbitMqPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(RabbitMqOptions opt)
    {
        var factory = new ConnectionFactory
        {
            HostName = opt.Host,
            Port = opt.Port,
            UserName = opt.Username,
            Password = opt.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void EnsureExchange(string exchange)
    {
        _channel.ExchangeDeclare(exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public void Publish(string exchange, string routingKey, string payloadJson, Guid? messageId = null)
    {
        EnsureExchange(exchange);

        var body = Encoding.UTF8.GetBytes(payloadJson);

        var props = _channel.CreateBasicProperties();
        props.DeliveryMode = 2; // persistent
        if (messageId is { } id)
        {
            props.MessageId = id.ToString();
        }

        _channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: props,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
