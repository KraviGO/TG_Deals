namespace Payments.UseCases.Wallet.ReserveForDeal;

public sealed record ReserveForDealCommand(Guid DealId, Guid AdvertiserUserId, Guid PublisherUserId, decimal Amount, string Currency);
