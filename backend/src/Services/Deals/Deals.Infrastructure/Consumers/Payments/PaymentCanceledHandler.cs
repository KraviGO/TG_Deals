using System.Text.Json;
using Deals.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Deals.Infrastructure.Consumers.Payments;

public sealed class PaymentCanceledHandler : IEventHandler
{
    public string RoutingKey => "payments.payment.canceled.v1";

    private readonly DealsDbContext _db;

    public PaymentCanceledHandler(DealsDbContext db) => _db = db;

    public async Task HandleAsync(string payloadJson, CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(payloadJson);
        var root = doc.RootElement;

        var paymentId = root.GetProperty("paymentId").GetGuid();
        var dealId = root.GetProperty("dealId").GetGuid();

        var deal = await _db.DealsSet.FirstOrDefaultAsync(x => x.DealId == dealId && x.PaymentId == paymentId, ct);
        if (deal is null) return;

        deal.SetPaymentState("Canceled", DateTimeOffset.UtcNow);
        await _db.SaveChangesAsync(ct);
    }
}
