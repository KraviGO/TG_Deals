namespace Payments.UseCases.Abstractions.YooKassa;

public sealed record YooKassaCreatePaymentRequest(
    decimal Amount,
    string Currency,
    string Description,
    string ReturnUrl
);
