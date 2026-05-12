namespace Payments.UseCases.Wallet.InternalCreditWallet;

public sealed record InternalCreditWalletResult(
    string Currency,
    decimal Available,
    decimal Reserved,
    decimal Total
);
