namespace ChannelCatalog.UseCases.Channels.SearchChannels;

public sealed record SearchChannelsResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string IntakeMode,
    string OwnershipStatus
);
