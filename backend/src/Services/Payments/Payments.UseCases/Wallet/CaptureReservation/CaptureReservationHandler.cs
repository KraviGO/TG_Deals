using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.CaptureReservation;

public sealed class CaptureReservationHandler
{
    private readonly IPaymentsDbContext _db;

    public CaptureReservationHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result> Handle(CaptureReservationCommand cmd, CancellationToken ct)
    {
        if (cmd.DealId == Guid.Empty) return Result.Fail(Errors.Validation);

        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.DealId == cmd.DealId, ct);
        if (r is null) return Result.Fail(Errors.NotFound);
        if (r.Status != ReservationStatus.Reserved) return Result.Ok();

        var wallet = await _db.Wallets.FirstAsync(x => x.UserId == r.UserId, ct);

        r.MarkCaptured(DateTimeOffset.UtcNow);
        wallet.CaptureFunds(r.Amount, DateTimeOffset.UtcNow);

        await _db.AddWalletTransactionAsync(WalletTransaction.CreateCapture(r.UserId, r.DealId, r.Amount, r.Currency, DateTimeOffset.UtcNow), ct);
        await _db.SaveChangesAsync(ct);

        // TODO: outbox wallet.reservation.captured.v1
        return Result.Ok();
    }
}
