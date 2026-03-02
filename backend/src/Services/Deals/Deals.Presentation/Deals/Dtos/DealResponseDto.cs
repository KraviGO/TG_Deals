namespace Deals.Presentation.Deals.Dtos;

public sealed record DealResponseDto(
    Guid DealId,
    Guid ChannelId,
    string Status,
    DateTimeOffset DesiredPublishAtUtc,
    DateTimeOffset CreatedAt
);
