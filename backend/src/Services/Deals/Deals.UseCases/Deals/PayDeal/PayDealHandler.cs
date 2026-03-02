using Deals.Entities.Deals;
using Deals.UseCases.Abstractions.Clock;
using Deals.UseCases.Abstractions.Payments;
using Deals.UseCases.Abstractions.Persistence;
using Deals.UseCases.Common;

namespace Deals.UseCases.Deals.PayDeal;

public sealed class PayDealHandler
{
    private readonly IDealsDbContext _db;
    private readonly IPaymentsClient _payments;
    private readonly IClock _clock;

    public PayDealHandler(IDealsDbContext db, IPaymentsClient payments, IClock clock)
    {
        _db = db;
        _payments = payments;
        _clock = clock;
    }

    public async Task<Result<PayDealResult>> Handle(PayDealCommand cmd, CancellationToken ct)
    {
        var deal = await _db.FindDealAsync(cmd.DealId, ct);
        if (deal is null) return Result<PayDealResult>.Fail(Errors.NotFound);

        if (deal.AdvertiserUserId != cmd.AdvertiserUserId)
            return Result<PayDealResult>.Fail(Errors.Forbidden);

        if (deal.PaymentId is not null)
            return Result<PayDealResult>.Fail(Errors.InvalidState);

        if (deal.Status != DealStatus.Accepted)
            return Result<PayDealResult>.Fail(Errors.InvalidState);

        var payment = await _payments.CreatePaymentAsync(deal.DealId, deal.AdvertiserUserId, cmd.Amount, cmd.Currency, ct);

        deal.AttachPayment(payment.PaymentId, _clock.UtcNow);
        await _db.SaveChangesAsync(ct);

        return Result<PayDealResult>.Ok(new PayDealResult(payment.PaymentId, payment.ConfirmationUrl));
    }
}
