namespace Deals.UseCases.Deals.GetPublisherDeals;

public sealed record GetPublisherDealsResult(
    Guid DealId,
    Guid ChannelId,
    Guid AdvertiserUserId,
    string Status,
    string FundingStatus,
    Guid? ReservationId,
    decimal Amount,
    string Currency,
    string PostText,
    string? PostUrl,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset DesiredPublishAtUtc,
    DateTimeOffset CreatedAt
);
