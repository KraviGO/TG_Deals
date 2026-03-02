namespace Deals.UseCases.Deals.CreateDeal;

public sealed record CreateDealCommand(
    Guid AdvertiserUserId,
    Guid ChannelId,
    string PostText,
    DateTimeOffset DesiredPublishAtUtc
);
