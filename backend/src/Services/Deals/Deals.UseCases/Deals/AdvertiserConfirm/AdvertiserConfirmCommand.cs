namespace Deals.UseCases.Deals.AdvertiserConfirm;

public sealed record AdvertiserConfirmCommand(Guid AdvertiserUserId, Guid DealId);
