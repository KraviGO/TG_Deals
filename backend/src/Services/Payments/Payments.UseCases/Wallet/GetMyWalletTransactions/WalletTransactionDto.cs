namespace Payments.UseCases.Wallet.GetMyWalletTransactions;

public sealed record WalletTransactionDto(
    Guid TxId,
    string Type,
    decimal Amount,
    string Currency,
    Guid? DealId,
    Guid? TopUpId,
    DateTimeOffset CreatedAt
);
