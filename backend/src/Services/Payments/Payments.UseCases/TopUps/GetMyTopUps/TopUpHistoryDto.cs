namespace Payments.UseCases.TopUps.GetMyTopUps;

public sealed record TopUpHistoryDto(
    Guid TopUpId,
    string YooKassaPaymentId,
    decimal Amount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
