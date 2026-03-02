namespace Payments.UseCases.Payments.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid DealId,
    Guid AdvertiserUserId,
    decimal Amount,
    string Currency
);
