namespace Payments.UseCases.Payments.CreatePayment;

public sealed record CreatePaymentResult(Guid PaymentId, string ConfirmationUrl);
