using System.Text.Json;
using Deals.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Deals.Infrastructure.Consumers.Payments;

public sealed class PaymentAuthorizedHandler : IEventHandler
{
    public string RoutingKey => "payments.payment.authorized.v1";

    private readonly DealsDbContext _db;

    public PaymentAuthorizedHandler(DealsDbContext db) => _db = db;

    public async Task HandleAsync(string payloadJson, CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(payloadJson);
        var root = doc.RootElement;

        var paymentId = root.GetProperty("paymentId").GetGuid();
        var dealId = root.GetProperty("dealId").GetGuid();

        var deal = await _db.DealsSet.FirstOrDefaultAsync(x => x.DealId == dealId && x.PaymentId == paymentId, ct);
        if (deal is null) return;

        deal.SetPaymentState("InEscrow", DateTimeOffset.UtcNow);
        await _db.SaveChangesAsync(ct);
    }
}
