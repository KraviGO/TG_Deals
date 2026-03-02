namespace Marketplace.Contracts.Publishers;

public sealed record ChannelRegisteredV1(
    Guid ChannelId,
    Guid PublisherUserId,
    string TelegramChannelId,
    string Title,
    string IntakeMode,
    string OwnershipStatus,
    DateTimeOffset OccurredAtUtc
);
