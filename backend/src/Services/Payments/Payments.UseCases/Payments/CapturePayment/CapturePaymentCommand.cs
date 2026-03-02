namespace Payments.UseCases.Payments.CapturePayment;

public sealed record CapturePaymentCommand(Guid PaymentId);
