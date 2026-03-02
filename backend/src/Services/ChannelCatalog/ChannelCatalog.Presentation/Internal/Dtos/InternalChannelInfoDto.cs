namespace ChannelCatalog.Presentation.Internal.Dtos;

public sealed record InternalChannelInfoDto(
    Guid ChannelId,
    Guid PublisherUserId,
    string IntakeMode,
    string OwnershipStatus
);
