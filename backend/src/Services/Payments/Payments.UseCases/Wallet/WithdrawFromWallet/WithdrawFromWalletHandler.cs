using Microsoft.EntityFrameworkCore;
using Payments.Entities.Wallet;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

namespace Payments.UseCases.Wallet.WithdrawFromWallet;

public sealed class WithdrawFromWalletHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IClock _clock;

    public WithdrawFromWalletHandler(IPaymentsDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<WithdrawFromWalletResult>> Handle(WithdrawFromWalletCommand cmd, CancellationToken ct)
    {
        if (cmd.UserId == Guid.Empty) return Result<WithdrawFromWalletResult>.Fail(Errors.Validation);
        if (cmd.Amount <= 0) return Result<WithdrawFromWalletResult>.Fail(Errors.Validation);
        if (!string.Equals(cmd.Currency, "RUB", StringComparison.OrdinalIgnoreCase))
            return Result<WithdrawFromWalletResult>.Fail("CurrencyNotSupported");

        var wallet = await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == cmd.UserId, ct);
        if (wallet is null) return Result<WithdrawFromWalletResult>.Fail(Errors.NotFound);

        if (!string.Equals(wallet.Currency, cmd.Currency, StringComparison.OrdinalIgnoreCase))
            return Result<WithdrawFromWalletResult>.Fail("CurrencyMismatch");

        var now = _clock.UtcNow;

        try
        {
            wallet.WithdrawFunds(cmd.Amount, now);
        }
        catch (InvalidOperationException ex) when (ex.Message == "InsufficientFunds")
        {
            return Result<WithdrawFromWalletResult>.Fail("InsufficientFunds");
        }

        await _db.AddWalletTransactionAsync(
            WalletTransaction.CreateWithdrawal(cmd.UserId, cmd.Amount, wallet.Currency, now), ct);

        await _db.SaveChangesAsync(ct);

        return Result<WithdrawFromWalletResult>.Ok(new WithdrawFromWalletResult(
            wallet.Currency,
            wallet.Available,
            wallet.Reserved,
            wallet.Available + wallet.Reserved));
    }
}
