namespace Payments.Presentation.InternalWallet.Dtos;

public sealed record ReserveRequestDto(Guid DealId, Guid AdvertiserUserId, decimal Amount, string Currency);
