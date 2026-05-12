using Marketplace.Messaging.Outbox;
using Marketplace.Contracts.Common;
using System.Text.Json;
using Publishers.Infrastructure.Persistence;
using Publishers.UseCases.Abstractions.Messaging;

namespace Publishers.Infrastructure.Outbox;

/// <summary>
/// Сохраняет integration event в outbox текущей транзакции.
/// </summary>
public sealed class EfOutboxWriter : IOutboxWriter
{
    private readonly PublishersDbContext _db;

    public EfOutboxWriter(PublishersDbContext db) => _db = db;

    public async Task EnqueueAsync<T>(string exchange, string routingKey, string eventType, int schemaVersion, T payload, string? correlationId, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var messageId = Guid.NewGuid();

        // Envelope хранит payload и метаданные доставки.
        var envelope = new EventEnvelope<T>(
            MessageId: messageId,
            OccurredAt: now,
            EventType: eventType,
            SchemaVersion: schemaVersion,
            CorrelationId: correlationId,
            Payload: payload
        );

        var entity = new OutboxMessage
        {
            Id = messageId,
            OccurredAt = now,
            EventType = eventType,
            Version = schemaVersion,
            Exchange = exchange,
            RoutingKey = routingKey,
            PayloadJson = JsonSerializer.Serialize(envelope),
            Status = OutboxMessageStatus.Pending,
            AttemptCount = 0
        };

        await _db.OutboxMessages.AddAsync(entity, ct);
    }
}
