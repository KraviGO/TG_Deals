namespace Deals.UseCases.Deals.PayDeal;

public sealed record PayDealCommand(Guid AdvertiserUserId, Guid DealId, decimal Amount, string Currency);
