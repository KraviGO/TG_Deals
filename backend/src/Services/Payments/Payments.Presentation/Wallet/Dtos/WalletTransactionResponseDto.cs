namespace Payments.Presentation.Wallet.Dtos;

public sealed record WalletTransactionResponseDto(
    Guid TxId,
    string Type,
    decimal Amount,
    string Currency,
    Guid? DealId,
    Guid? TopUpId,
    DateTimeOffset CreatedAt
);
