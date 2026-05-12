namespace Deals.UseCases.Deals.GetMyDeals;

public sealed record GetMyDealsResult(
    Guid DealId,
    Guid ChannelId,
    string Status,
    string FundingStatus,
    Guid? ReservationId,
    decimal Amount,
    string Currency,
    string? PostUrl,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset DesiredPublishAtUtc,
    DateTimeOffset CreatedAt
);
