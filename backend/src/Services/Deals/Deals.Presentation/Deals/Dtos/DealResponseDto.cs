namespace Deals.Presentation.Deals.Dtos;

public sealed record DealResponseDto(
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
