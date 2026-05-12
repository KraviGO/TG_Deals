namespace Deals.UseCases.Abstractions.Wallet;

public sealed record WalletReservationResult(Guid ReservationId, string Status);

public interface IWalletClient
{
    Task<WalletReservationResult> ReserveForDealAsync(
        Guid dealId,
        Guid advertiserUserId,
        Guid publisherUserId,
        decimal amount,
        string currency,
        CancellationToken ct);

    Task ReleaseReservationAsync(Guid dealId, CancellationToken ct);

    Task CaptureReservationAsync(Guid dealId, CancellationToken ct);
}
