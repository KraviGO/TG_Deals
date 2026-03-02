namespace Deals.UseCases.Deals.GetPublisherDeals;

public sealed record GetPublisherDealsResult(
    Guid DealId,
    Guid ChannelId,
    Guid AdvertiserUserId,
    string Status,
    DateTimeOffset DesiredPublishAtUtc,
    DateTimeOffset CreatedAt
);
