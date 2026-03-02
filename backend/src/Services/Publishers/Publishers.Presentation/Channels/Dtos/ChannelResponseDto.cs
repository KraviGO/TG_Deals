namespace Publishers.Presentation.Channels.Dtos;

public sealed record ChannelResponseDto(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string IntakeMode,
    string OwnershipStatus
);
