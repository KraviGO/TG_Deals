namespace Deals.UseCases.Deals.GetMyDeals;

public sealed record GetMyDealsResult(
    Guid DealId,
    Guid ChannelId,
    string Status,
    DateTimeOffset DesiredPublishAtUtc,
    DateTimeOffset CreatedAt
);
