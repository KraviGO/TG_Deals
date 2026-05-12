namespace Payments.Presentation.Wallet.Dtos;

public sealed record TopUpHistoryResponseDto(
    Guid TopUpId,
    string YooKassaPaymentId,
    decimal Amount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
