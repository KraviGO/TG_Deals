namespace Payments.UseCases.Abstractions.Messaging;

public interface IOutboxWriter
{
    Task EnqueueAsync(OutboxEnvelope message, CancellationToken ct);
}

public sealed record OutboxEnvelope(
    string EventType,
    int Version,
    string Exchange,
    string RoutingKey,
    string PayloadJson,
    DateTimeOffset OccurredAtUtc
);
