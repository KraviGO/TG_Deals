namespace Payments.UseCases.Abstractions.YooKassa;

public sealed record YooKassaCreatePaymentResult(
    string YooKassaPaymentId,
    string Status,
    string? ConfirmationUrl
);
