using Microsoft.EntityFrameworkCore;
using Payments.Entities.TopUps;
using Payments.Entities.Wallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.TopUps.ProcessTopUpWebhook;

/// <summary>
/// Применяет webhook YooKassa к пополнению.
/// </summary>
public sealed class ProcessTopUpWebhookHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public ProcessTopUpWebhookHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result> Handle(ProcessTopUpWebhookCommand cmd, CancellationToken ct)
    {
        var evt = cmd.Notification.Event;
        var yooId = cmd.Notification.Object.Id;

        var topup = await _db.TopUps.FirstOrDefaultAsync(x => x.YooKassaPaymentId == yooId, ct);
        if (topup is null) return Result.Ok();

        if (evt == "payment.succeeded")
        {
            if (topup.Status == TopUpStatus.Succeeded) return Result.Ok();

            var now = _clock.UtcNow;
            topup.MarkSucceeded(now);

            // Успешное пополнение увеличивает Available рекламодателя.
            var wallet = await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == topup.UserId, ct);
            if (wallet is null)
            {
                wallet = WalletEntity.Create(topup.UserId, topup.Currency, now);
                await _db.AddWalletAsync(wallet, ct);
            }

            wallet.Credit(topup.Amount, now);

            await _db.AddWalletTransactionAsync(WalletTransaction.CreateTopUp(
                topup.UserId,
                topup.TopUpId,
                topup.Amount,
                topup.Currency,
                now), ct);

            await _db.SaveChangesAsync(ct);

            return Result.Ok();
        }

        if (evt == "payment.canceled")
        {
            if (topup.Status == TopUpStatus.Canceled) return Result.Ok();
            topup.MarkCanceled(_clock.UtcNow);
            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }

        return Result.Ok();
    }
}
