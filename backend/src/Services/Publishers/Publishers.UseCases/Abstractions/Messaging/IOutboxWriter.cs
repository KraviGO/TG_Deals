namespace Publishers.UseCases.Abstractions.Messaging;

public interface IOutboxWriter
{
    Task EnqueueAsync<T>(string exchange, string routingKey, string eventType, int schemaVersion, T payload, string? correlationId, CancellationToken ct);
}
