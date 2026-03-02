using Marketplace.Messaging.Outbox;
using Publishers.Infrastructure.Persistence;
using Publishers.UseCases.Abstractions.Messaging;

namespace Publishers.Infrastructure.Outbox;

public sealed class EfOutboxWriter : IOutboxWriter
{
    private readonly PublishersDbContext _db;

    public EfOutboxWriter(PublishersDbContext db) => _db = db;

    public async Task EnqueueAsync(OutboxEnvelope message, CancellationToken ct)
    {
        var entity = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredAt = message.OccurredAtUtc,
            EventType = message.EventType,
            Version = message.Version,
            Exchange = message.Exchange,
            RoutingKey = message.RoutingKey,
            PayloadJson = message.PayloadJson,
            Status = OutboxMessageStatus.Pending,
            AttemptCount = 0
        };

        await _db.OutboxMessages.AddAsync(entity, ct);
    }
}
