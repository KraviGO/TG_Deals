using System.Text.Json;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Messaging;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Common;

namespace Payments.UseCases.Payments.ProcessWebhook;

public sealed class ProcessWebhookHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;

    public ProcessWebhookHandler(IPaymentsDbContext db, IClock clock, IOutboxWriter outbox)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
    }

    public async Task<Result> Handle(ProcessWebhookCommand cmd, CancellationToken ct)
    {
        var evt = cmd.Notification.Event;
        var yooId = cmd.Notification.Object.Id;

        var p = await _db.FindByYooKassaIdAsync(yooId, ct);
        if (p is null) return Result.Ok();

        string? routingKey = null;
        string? eventType = null;

        if (evt == "payment.waiting_for_capture")
        {
            if (p.Status != Entities.Payments.PaymentStatus.Authorized)
            {
                p.MarkAuthorized(_clock.UtcNow);
                routingKey = "payments.payment.authorized.v1";
                eventType = "PaymentAuthorized";
            }
        }
        else if (evt == "payment.succeeded")
        {
            if (p.Status != Entities.Payments.PaymentStatus.Captured)
            {
                p.MarkCaptured(_clock.UtcNow);
                routingKey = "payments.payment.captured.v1";
                eventType = "PaymentCaptured";
            }
        }
        else if (evt == "payment.canceled")
        {
            if (p.Status != Entities.Payments.PaymentStatus.Canceled)
            {
                p.MarkCanceled(_clock.UtcNow);
                routingKey = "payments.payment.canceled.v1";
                eventType = "PaymentCanceled";
            }
        }
        else
        {
            return Result.Ok();
        }

        if (routingKey is not null && eventType is not null)
        {
            var payload = JsonSerializer.Serialize(new
            {
                paymentId = p.PaymentId,
                dealId = p.DealId,
                advertiserUserId = p.AdvertiserUserId,
                amount = p.Amount,
                currency = p.Currency,
                status = p.Status.ToString(),
                occurredAtUtc = _clock.UtcNow
            });

            await _outbox.EnqueueAsync(new OutboxEnvelope(
                EventType: eventType,
                Version: 1,
                Exchange: "marketplace.events",
                RoutingKey: routingKey,
                PayloadJson: payload,
                OccurredAtUtc: _clock.UtcNow
            ), ct);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
