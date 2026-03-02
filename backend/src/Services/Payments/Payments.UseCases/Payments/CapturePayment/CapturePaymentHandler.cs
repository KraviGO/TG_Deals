using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Abstractions.YooKassa;
using Payments.UseCases.Common;

namespace Payments.UseCases.Payments.CapturePayment;

public sealed class CapturePaymentHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IYooKassaClient _yoo;
    private readonly IClock _clock;

    public CapturePaymentHandler(IPaymentsDbContext db, IYooKassaClient yoo, IClock clock)
    {
        _db = db;
        _yoo = yoo;
        _clock = clock;
    }

    public async Task<Result> Handle(CapturePaymentCommand cmd, CancellationToken ct)
    {
        var p = await _db.FindByPaymentIdAsync(cmd.PaymentId, ct);
        if (p is null) return Result.Fail(Errors.NotFound);

        if (p.Status != Entities.Payments.PaymentStatus.Authorized)
            return Result.Fail(Errors.InvalidState);

        await _yoo.CaptureAsync(p.YooKassaPaymentId, Guid.NewGuid(), ct);
        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
