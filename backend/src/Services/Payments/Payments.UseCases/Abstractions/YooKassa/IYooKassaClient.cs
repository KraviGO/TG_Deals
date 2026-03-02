namespace Payments.UseCases.Abstractions.YooKassa;

public interface IYooKassaClient
{
    Task<YooKassaCreatePaymentResult> CreateTwoStagePaymentAsync(
        YooKassaCreatePaymentRequest req,
        Guid idempotenceKey,
        CancellationToken ct);

    Task CaptureAsync(string yooKassaPaymentId, Guid idempotenceKey, CancellationToken ct);
}
