using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.ReserveForDeal;

/// <summary>
/// Резервирует деньги рекламодателя под сделку.
/// </summary>
public sealed class ReserveForDealHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public ReserveForDealHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<ReserveForDealResult>> Handle(ReserveForDealCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty || cmd.AdvertiserUserId == Guid.Empty || cmd.PublisherUserId == Guid.Empty)
            return Result<ReserveForDealResult>.Fail(Errors.Validation);
        if (cmd.Amount <= 0) return Result<ReserveForDealResult>.Fail(Errors.Validation);

        var existing = await _db.Reservations.FirstOrDefaultAsync(x => x.DealId == cmd.DealId, ct);
        if (existing is not null)
        {
            // Повторный internal-вызов возвращает существующий резерв.
            return Result<ReserveForDealResult>.Ok(new ReserveForDealResult(existing.ReservationId, existing.Status.ToString()));
        }

        var now = _clock.UtcNow;

        var wallet = await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == cmd.AdvertiserUserId, ct);
        if (wallet is null)
        {
            wallet = WalletEntity.Create(cmd.AdvertiserUserId, cmd.Currency, now);
            await _db.AddWalletAsync(wallet, ct);
            await _db.SaveChangesAsync(ct);
        }

        if (!string.Equals(wallet.Currency, cmd.Currency, StringComparison.OrdinalIgnoreCase))
            return Result<ReserveForDealResult>.Fail("CurrencyMismatch");

        try
        {
            // Резерв переносит деньги из Available в Reserved.
            wallet.ReserveFunds(cmd.Amount, now);
            var reservation = Reservation.Create(cmd.DealId, cmd.AdvertiserUserId, cmd.PublisherUserId, cmd.Amount, cmd.Currency, now);
            await _db.AddReservationAsync(reservation, ct);
            await _db.AddWalletTransactionAsync(WalletTransaction.CreateReserve(cmd.AdvertiserUserId, cmd.DealId, cmd.Amount, cmd.Currency, now), ct);

            await _db.SaveChangesAsync(ct);

            return Result<ReserveForDealResult>.Ok(new ReserveForDealResult(reservation.ReservationId, reservation.Status.ToString()));
        }
        catch (InvalidOperationException ex) when (ex.Message == "InsufficientFunds")
        {
            return Result<ReserveForDealResult>.Fail("InsufficientFunds");
        }
    }
}
