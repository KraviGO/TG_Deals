namespace Deals.Presentation.Deals.Dtos;

public sealed record CreateDealRequestDto(
    Guid ChannelId,
    string PostText,
    DateTimeOffset DesiredPublishAtUtc
);
