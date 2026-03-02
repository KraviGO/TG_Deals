namespace Marketplace.Messaging.RabbitMq;

public sealed record RabbitMqOptions
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = "marketplace";
    public string Password { get; init; } = "marketplace";
}
