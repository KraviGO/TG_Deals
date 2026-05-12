namespace ChannelCatalog.UseCases.Channels.SearchChannels;

public sealed record SearchChannelsResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string Topic,
    string Language,
    decimal PricePerPostRub,
    string IntakeMode,
    string OwnershipStatus
);
