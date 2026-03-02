using Microsoft.EntityFrameworkCore;
using Payments.Entities.TopUps;
using Payments.Entities.Wallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Common;

namespace Payments.UseCases.TopUps.ProcessTopUpWebhook;

public sealed class ProcessTopUpWebhookHandler
{
    private readonly IPaymentsDbContext _db;

    public ProcessTopUpWebhookHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result> Handle(ProcessTopUpWebhookCommand cmd, CancellationToken ct)
    {
        var evt = cmd.Notification.Event;
        var yooId = cmd.Notification.Object.Id;

        var topup = await _db.TopUps.FirstOrDefaultAsync(x => x.YooKassaPaymentId == yooId, ct);
        if (topup is null) return Result.Ok();

        if (evt == "payment.succeeded")
        {
            if (topup.Status == TopUpStatus.Succeeded) return Result.Ok();

            topup.MarkSucceeded(DateTimeOffset.UtcNow);

            var wallet = await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == topup.UserId, ct);
            if (wallet is null)
            {
                wallet = WalletEntity.Create(topup.UserId, topup.Currency, DateTimeOffset.UtcNow);
                await _db.AddWalletAsync(wallet, ct);
            }

            wallet.Credit(topup.Amount, DateTimeOffset.UtcNow);

            await _db.AddWalletTransactionAsync(WalletTransaction.CreateTopUp(topup.UserId, topup.TopUpId, topup.Amount, topup.Currency, DateTimeOffset.UtcNow), ct);
            await _db.SaveChangesAsync(ct);

            // TODO: outbox wallet.topup.succeeded.v1
            return Result.Ok();
        }

        if (evt == "payment.canceled")
        {
            if (topup.Status == TopUpStatus.Canceled) return Result.Ok();
            topup.MarkCanceled(DateTimeOffset.UtcNow);
            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }

        return Result.Ok();
    }
}
