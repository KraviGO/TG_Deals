using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.ReleaseReservation;

public sealed class ReleaseReservationHandler
{
    private readonly IPaymentsDbContext _db;

    public ReleaseReservationHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result> Handle(ReleaseReservationCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty) return Result.Fail(Errors.Validation);

        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.DealId == cmd.DealId, ct);
        if (r is null) return Result.Ok();
        if (r.Status != ReservationStatus.Reserved) return Result.Ok();

        var wallet = await _db.Wallets.FirstAsync(x => x.UserId == r.UserId, ct);

        r.MarkReleased(DateTimeOffset.UtcNow);
        wallet.ReleaseFunds(r.Amount, DateTimeOffset.UtcNow);

        await _db.AddWalletTransactionAsync(WalletTransaction.CreateRelease(r.UserId, r.DealId, r.Amount, r.Currency, DateTimeOffset.UtcNow), ct);
        await _db.SaveChangesAsync(ct);

        // TODO: outbox wallet.reservation.released.v1
        return Result.Ok();
    }
}
