using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using WalletEntity = Payments.Entities.Wallet.Wallet;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.InternalCreditWallet;

/// <summary>
/// Начисляет деньги рекламодателю через internal dev endpoint.
/// </summary>
public sealed class InternalCreditWalletHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public InternalCreditWalletHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<InternalCreditWalletResult>> Handle(InternalCreditWalletCommand cmd, CancellationToken ct)
    {
        if (cmd.UserId == Guid.Empty || cmd.Amount <= 0 || string.IsNullOrWhiteSpace(cmd.Currency))
            return Result<InternalCreditWalletResult>.Fail(Errors.Validation);

        var now = _clock.UtcNow;
        var currency = cmd.Currency.Trim().ToUpperInvariant();

        var wallet = await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == cmd.UserId, ct);
        if (wallet is null)
        {
            wallet = WalletEntity.Create(cmd.UserId, currency, now);
            await _db.AddWalletAsync(wallet, ct);
        }
        else if (!string.Equals(wallet.Currency, currency, StringComparison.OrdinalIgnoreCase))
        {
            return Result<InternalCreditWalletResult>.Fail("CurrencyMismatch");
        }

        wallet.Credit(cmd.Amount, now);
        await _db.AddWalletTransactionAsync(WalletTransaction.CreateManualCredit(cmd.UserId, cmd.Amount, currency, now), ct);
        await _db.SaveChangesAsync(ct);

        return Result<InternalCreditWalletResult>.Ok(new InternalCreditWalletResult(
            Currency: wallet.Currency,
            Available: wallet.Available,
            Reserved: wallet.Reserved,
            Total: wallet.Available + wallet.Reserved));
    }
}
