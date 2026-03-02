namespace Deals.UseCases.Deals.CancelDeal;

public sealed record CancelDealCommand(Guid AdvertiserUserId, Guid DealId);
