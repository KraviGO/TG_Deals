namespace Payments.UseCases.Wallet.ReserveForDeal;

public sealed record ReserveForDealCommand(Guid DealId, Guid AdvertiserUserId, decimal Amount, string Currency);
