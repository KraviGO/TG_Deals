using Microsoft.Extensions.Options;
using Payments.Entities.Payments;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Abstractions.YooKassa;
using Payments.UseCases.Common;

namespace Payments.UseCases.Payments.CreatePayment;

public sealed class CreatePaymentHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IYooKassaClient _yoo;
    private readonly IClock _clock;
    private readonly YooKassaOptions _opt;

    public CreatePaymentHandler(IPaymentsDbContext db, IYooKassaClient yoo, IClock clock, IOptions<YooKassaOptions> opt)
    {
        _db = db;
        _yoo = yoo;
        _clock = clock;
        _opt = opt.Value;
    }

    public async Task<Result<CreatePaymentResult>> Handle(CreatePaymentCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty || cmd.AdvertiserUserId == Guid.Empty) return Result<CreatePaymentResult>.Fail(Errors.Validation);
        if (cmd.Amount <= 0) return Result<CreatePaymentResult>.Fail(Errors.Validation);

        var payment = Payment.Create(cmd.DealId, cmd.AdvertiserUserId, cmd.Amount, cmd.Currency, _clock.UtcNow);

        var idempotenceKey = payment.PaymentId;
        var yooRes = await _yoo.CreateTwoStagePaymentAsync(
            new YooKassaCreatePaymentRequest(
                Amount: cmd.Amount,
                Currency: cmd.Currency,
                Description: $"Deal {cmd.DealId}",
                ReturnUrl: _opt.ReturnUrl
            ),
            idempotenceKey,
            ct);

        if (string.IsNullOrWhiteSpace(yooRes.ConfirmationUrl))
            return Result<CreatePaymentResult>.Fail("NoConfirmationUrl");

        payment.SetYooKassaInfo(yooRes.YooKassaPaymentId, yooRes.ConfirmationUrl, _clock.UtcNow);

        await _db.AddPaymentAsync(payment, ct);
        await _db.SaveChangesAsync(ct);

        return Result<CreatePaymentResult>.Ok(new CreatePaymentResult(payment.PaymentId, yooRes.ConfirmationUrl!));
    }
}
