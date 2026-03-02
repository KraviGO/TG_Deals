namespace Marketplace.Messaging.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset OccurredAt { get; set; }
    public string EventType { get; set; } = default!;
    public int Version { get; set; } = 1;

    public string Exchange { get; set; } = "marketplace.events";
    public string RoutingKey { get; set; } = default!;
    public string PayloadJson { get; set; } = default!;

    public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;
    public int AttemptCount { get; set; } = 0;
    public string? LastError { get; set; }

    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? NextAttemptAt { get; set; }
}
