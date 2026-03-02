namespace Deals.UseCases.Abstractions.Payments;

public sealed record PaymentCreateResult(Guid PaymentId, string ConfirmationUrl);

public interface IPaymentsClient
{
    Task<PaymentCreateResult> CreatePaymentAsync(Guid dealId, Guid advertiserUserId, decimal amount, string currency, CancellationToken ct);
}
