namespace Payments.Presentation.InternalWallet.Dtos;

public sealed record ReserveRequestDto(Guid DealId, Guid AdvertiserUserId, Guid PublisherUserId, decimal Amount, string Currency);
