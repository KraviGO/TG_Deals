using Payments.Entities.TopUps;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Abstractions.YooKassa;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;
using Microsoft.Extensions.Options;

namespace Payments.UseCases.TopUps.CreateTopUp;

/// <summary>
/// Создает платеж YooKassa для пополнения кошелька.
/// </summary>
public sealed class CreateTopUpHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IYooKassaClient _yoo;
    private readonly YooKassaOptions _opt;

    public CreateTopUpHandler(IPaymentsDbContext db, IYooKassaClient yoo, IOptions<YooKassaOptions> opt)
    {
        _db = db;
        _yoo = yoo;
        _opt = opt.Value;
    }

    public async Task<Result<CreateTopUpResult>> Handle(CreateTopUpCommand cmd, CancellationToken ct)
    {
        if (cmd.UserId == Guid.Empty) return Result<CreateTopUpResult>.Fail(Errors.Validation);
        if (cmd.Amount <= 0) return Result<CreateTopUpResult>.Fail(Errors.Validation);
        if (string.IsNullOrWhiteSpace(cmd.Currency)) return Result<CreateTopUpResult>.Fail(Errors.Validation);

        var topup = TopUp.Create(cmd.UserId, cmd.Amount, cmd.Currency, DateTimeOffset.UtcNow);

        // Idempotence-Key равен TopUpId, чтобы повтор создания не дублировал платеж.
        var yoo = await _yoo.CreateTwoStagePaymentAsync(
            new YooKassaCreatePaymentRequest(
                Amount: cmd.Amount,
                Currency: cmd.Currency,
                Description: $"TopUp {topup.TopUpId}",
                ReturnUrl: _opt.ReturnUrl
            ),
            idempotenceKey: topup.TopUpId,
            ct);

        if (string.IsNullOrWhiteSpace(yoo.ConfirmationUrl))
            return Result<CreateTopUpResult>.Fail("NoConfirmationUrl");

        topup.SetYooKassa(yoo.YooKassaPaymentId, yoo.ConfirmationUrl, DateTimeOffset.UtcNow);

        await _db.AddTopUpAsync(topup, ct);
        await _db.SaveChangesAsync(ct);

        return Result<CreateTopUpResult>.Ok(new CreateTopUpResult(topup.TopUpId, yoo.ConfirmationUrl!));
    }
}
