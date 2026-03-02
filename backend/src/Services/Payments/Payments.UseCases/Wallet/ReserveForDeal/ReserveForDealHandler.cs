using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.ReserveForDeal;

public sealed class ReserveForDealHandler
{
    private readonly IPaymentsDbContext _db;

    public ReserveForDealHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result<ReserveForDealResult>> Handle(ReserveForDealCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty || cmd.AdvertiserUserId == Guid.Empty) return Result<ReserveForDealResult>.Fail(Errors.Validation);
        if (cmd.Amount <= 0) return Result<ReserveForDealResult>.Fail(Errors.Validation);

        var existing = await _db.Reservations.FirstOrDefaultAsync(x => x.DealId == cmd.DealId, ct);
        if (existing is not null)
            return Result<ReserveForDealResult>.Ok(new ReserveForDealResult(existing.ReservationId, existing.Status.ToString()));

        var wallet = await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == cmd.AdvertiserUserId, ct);
        if (wallet is null)
        {
            wallet = WalletEntity.Create(cmd.AdvertiserUserId, cmd.Currency, DateTimeOffset.UtcNow);
            await _db.AddWalletAsync(wallet, ct);
            await _db.SaveChangesAsync(ct);
        }

        if (!string.Equals(wallet.Currency, cmd.Currency, StringComparison.OrdinalIgnoreCase))
            return Result<ReserveForDealResult>.Fail("CurrencyMismatch");

        try
        {
            wallet.ReserveFunds(cmd.Amount, DateTimeOffset.UtcNow);
            var reservation = Reservation.Create(cmd.DealId, cmd.AdvertiserUserId, cmd.Amount, cmd.Currency, DateTimeOffset.UtcNow);
            await _db.AddReservationAsync(reservation, ct);
            await _db.AddWalletTransactionAsync(WalletTransaction.CreateReserve(cmd.AdvertiserUserId, cmd.DealId, cmd.Amount, cmd.Currency, DateTimeOffset.UtcNow), ct);

            await _db.SaveChangesAsync(ct);

            // TODO: outbox wallet.reservation.created.v1
            return Result<ReserveForDealResult>.Ok(new ReserveForDealResult(reservation.ReservationId, reservation.Status.ToString()));
        }
        catch (InvalidOperationException ex) when (ex.Message == "InsufficientFunds")
        {
            return Result<ReserveForDealResult>.Fail("InsufficientFunds");
        }
    }
}
