namespace ChannelCatalog.Infrastructure.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MessageId { get; set; } = default!;
    public string RoutingKey { get; set; } = default!;
    public DateTimeOffset ProcessedAt { get; set; }
}
