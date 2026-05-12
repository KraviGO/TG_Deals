namespace Marketplace.Messaging.RabbitMq;

/// <summary>
/// Настройки подключения к RabbitMQ.
/// Значения по умолчанию настроены для локального Docker-окружения.
/// </summary>
public sealed record RabbitMqOptions
{
    /// <summary>
    /// Адрес RabbitMQ-хоста.
    /// </summary>
    public string Host { get; init; } = "localhost";

    /// <summary>
    /// Порт AMQP-подключения.
    /// </summary>
    public int Port { get; init; } = 5672;

    /// <summary>
    /// Имя пользователя RabbitMQ.
    /// </summary>
    public string Username { get; init; } = "marketplace";

    /// <summary>
    /// Пароль пользователя RabbitMQ.
    /// </summary>
    public string Password { get; init; } = "marketplace";
}
