namespace Publishers.Presentation.Channels.Dtos;

public sealed record ChannelResponseDto(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string Topic,
    string Language,
    decimal PricePerPostRub,
    string IntakeMode,
    string OwnershipStatus
);
