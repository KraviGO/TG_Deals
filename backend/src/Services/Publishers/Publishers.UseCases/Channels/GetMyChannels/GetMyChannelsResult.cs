using Publishers.Entities.Channels;

namespace Publishers.UseCases.Channels.GetMyChannels;

public sealed record GetMyChannelsResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string Topic,
    string Language,
    decimal PricePerPostRub,
    IntakeMode IntakeMode,
    OwnershipStatus OwnershipStatus
);
