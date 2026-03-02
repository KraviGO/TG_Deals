using Microsoft.EntityFrameworkCore;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.GetMyWallet;

public sealed class GetMyWalletHandler
{
    private readonly IPaymentsDbContext _db;

    public GetMyWalletHandler(IPaymentsDbContext db) => _db = db;

    public async Task<Result<WalletDto>> Handle(GetMyWalletQuery q, CancellationToken ct)
    {
        if (q.UserId == Guid.Empty) return Result<WalletDto>.Fail(Errors.Validation);

        var w = await _db.Wallets.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == q.UserId, ct);

        if (w is null)
        {
            var now = DateTimeOffset.UtcNow;
            var wallet = WalletEntity.Create(q.UserId, "RUB", now);
            await _db.AddWalletAsync(wallet, ct);
            await _db.SaveChangesAsync(ct);

            return Result<WalletDto>.Ok(new WalletDto(wallet.Currency, wallet.Available, wallet.Reserved, wallet.Available + wallet.Reserved));
        }

        return Result<WalletDto>.Ok(new WalletDto(w.Currency, w.Available, w.Reserved, w.Available + w.Reserved));
    }
}
