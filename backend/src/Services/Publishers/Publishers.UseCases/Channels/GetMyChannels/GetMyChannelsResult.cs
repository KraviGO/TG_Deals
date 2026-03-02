using Publishers.Entities.Channels;

namespace Publishers.UseCases.Channels.GetMyChannels;

public sealed record GetMyChannelsResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    IntakeMode IntakeMode,
    OwnershipStatus OwnershipStatus
);
