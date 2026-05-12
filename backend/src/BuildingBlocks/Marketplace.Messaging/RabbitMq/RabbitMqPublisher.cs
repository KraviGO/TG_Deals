using System.Text;
using RabbitMQ.Client;

namespace Marketplace.Messaging.RabbitMq;

/// <summary>
/// Синхронный publisher JSON-сообщений в RabbitMQ topic exchange.
/// Экземпляр владеет AMQP connection и channel до вызова <see cref="Dispose"/>.
/// </summary>
public sealed class RabbitMqPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    /// <summary>
    /// Открывает подключение к RabbitMQ и канал публикации.
    /// </summary>
    /// <param name="opt">Параметры подключения к брокеру.</param>
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

    /// <summary>
    /// Создает durable topic exchange, если он еще не существует.
    /// Повторный вызов с теми же параметрами не меняет exchange.
    /// </summary>
    /// <param name="exchange">Имя exchange для интеграционных событий.</param>
    public void EnsureExchange(string exchange)
    {
        _channel.ExchangeDeclare(exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);
    }

    /// <summary>
    /// Публикует JSON-сообщение в exchange с указанным routing key.
    /// Сообщение получает persistent-флаг RabbitMQ.
    /// </summary>
    /// <param name="exchange">Exchange для публикации.</param>
    /// <param name="routingKey">Routing key для маршрутизации сообщения.</param>
    /// <param name="payloadJson">JSON события или envelope.</param>
    /// <param name="messageId">Идентификатор сообщения для трассировки и идемпотентной обработки.</param>
    public void Publish(string exchange, string routingKey, string payloadJson, Guid? messageId = null)
    {
        EnsureExchange(exchange);

        var body = Encoding.UTF8.GetBytes(payloadJson);

        var props = _channel.CreateBasicProperties();
        // DeliveryMode = 2 ставит persistent-флаг RabbitMQ для durable очередей.
        props.DeliveryMode = 2;
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

    /// <summary>
    /// Закрывает AMQP channel и connection.
    /// </summary>
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
