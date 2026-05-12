namespace ChannelCatalog.UseCases.Channels.GetChannelById;

public sealed record GetChannelByIdResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string Topic,
    string Language,
    decimal PricePerPostRub,
    string IntakeMode,
    string OwnershipStatus
);
