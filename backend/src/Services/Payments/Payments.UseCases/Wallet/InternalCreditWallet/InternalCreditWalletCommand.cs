namespace Payments.UseCases.Wallet.InternalCreditWallet;

public sealed record InternalCreditWalletCommand(
    Guid UserId,
    decimal Amount,
    string Currency
);
