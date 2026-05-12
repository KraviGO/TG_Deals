namespace Payments.UseCases.Wallet.WithdrawFromWallet;

public sealed record WithdrawFromWalletResult(
    string Currency,
    decimal Available,
    decimal Reserved,
    decimal Total
);
