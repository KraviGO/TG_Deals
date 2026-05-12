using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.ReleaseReservation;

/// <summary>
/// Возвращает резерв рекламодателю.
/// </summary>
public sealed class ReleaseReservationHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public ReleaseReservationHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result> Handle(ReleaseReservationCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty) return Result.Fail(Errors.Validation);

        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.DealId == cmd.DealId, ct);
        if (r is null) return Result.Ok();
        if (r.Status != ReservationStatus.Reserved) return Result.Ok();

        var now = _clock.UtcNow;
        var wallet = await _db.Wallets.FirstAsync(x => x.UserId == r.UserId, ct);

        // Release переносит деньги из Reserved в Available.
        r.MarkReleased(now);
        wallet.ReleaseFunds(r.Amount, now);

        await _db.AddWalletTransactionAsync(WalletTransaction.CreateRelease(r.UserId, r.DealId, r.Amount, r.Currency, now), ct);

        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
