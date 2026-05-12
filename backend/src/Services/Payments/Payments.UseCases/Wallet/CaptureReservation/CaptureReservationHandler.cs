using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payments.Entities.PublisherLedger;
using Payments.Entities.Wallet;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Fees;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.CaptureReservation;

/// <summary>
/// Списывает резерв и начисляет деньги паблишеру.
/// </summary>
public sealed class CaptureReservationHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;
    private readonly PlatformFeesOptions _fees;

    public CaptureReservationHandler(
        IPaymentsDbContext db,
        IClock clock,
        IOptions<PlatformFeesOptions> fees)
    {
        _db = db;
        _clock = clock;
        _fees = fees.Value;
    }

    public async Task<Result> Handle(CaptureReservationCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty) return Result.Fail(Errors.Validation);

        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.DealId == cmd.DealId, ct);
        if (r is null) return Result.Fail(Errors.NotFound);
        if (r.Status != ReservationStatus.Reserved) return Result.Ok();

        var now = _clock.UtcNow;
        var wallet = await _db.Wallets.FirstAsync(x => x.UserId == r.UserId, ct);

        r.MarkCaptured(now);
        wallet.CaptureFunds(r.Amount, now);

        await _db.AddWalletTransactionAsync(WalletTransaction.CreateCapture(r.UserId, r.DealId, r.Amount, r.Currency, now), ct);

        var existingLedger = await _db.PublisherLedgerEntries
            .FirstOrDefaultAsync(x => x.DealId == r.DealId, ct);

        if (existingLedger is null)
        {
            // Начисление создается один раз на сделку.
            var feeBps = Math.Clamp(_fees.PlatformFeeBps, 0, 10_000);
            var platformFee = RoundMoney(r.Amount * feeBps / 10_000m);
            var publisherAmount = RoundMoney(r.Amount - platformFee);

            var entry = PublisherLedgerEntry.CreateAccrual(
                dealId: r.DealId,
                publisherUserId: r.PublisherUserId,
                grossAmount: r.Amount,
                platformFeeAmount: platformFee,
                publisherAmount: publisherAmount,
                currency: r.Currency,
                nowUtc: now);

            await _db.AddPublisherLedgerEntryAsync(entry, ct);

            var publisherWallet = await _db.PublisherWallets
                .FirstOrDefaultAsync(x => x.PublisherUserId == r.PublisherUserId, ct);

            if (publisherWallet is null)
            {
                publisherWallet = global::Payments.Entities.PublisherLedger.PublisherWallet.Create(r.PublisherUserId, r.Currency, now);
                await _db.AddPublisherWalletAsync(publisherWallet, ct);
            }

            publisherWallet.AddAvailable(publisherAmount, now);
        }

        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }

    private static decimal RoundMoney(decimal amount)
        => Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}
