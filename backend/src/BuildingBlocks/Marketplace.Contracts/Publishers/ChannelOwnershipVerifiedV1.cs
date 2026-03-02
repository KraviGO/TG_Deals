namespace Marketplace.Contracts.Publishers;

public sealed record ChannelOwnershipVerifiedV1(
    Guid ChannelId,
    Guid PublisherUserId,
    DateTimeOffset OccurredAtUtc
);
