using Marketplace.Messaging.Outbox;
using Payments.Infrastructure.Persistence;
using Payments.UseCases.Abstractions.Messaging;

namespace Payments.Infrastructure.Outbox;

public sealed class EfOutboxWriter : IOutboxWriter
{
    private readonly PaymentsDbContext _db;

    public EfOutboxWriter(PaymentsDbContext db) => _db = db;

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
            AttemptCount = 0,
            NextAttemptAt = null
        };

        await _db.OutboxMessages.AddAsync(entity, ct);
    }
}
