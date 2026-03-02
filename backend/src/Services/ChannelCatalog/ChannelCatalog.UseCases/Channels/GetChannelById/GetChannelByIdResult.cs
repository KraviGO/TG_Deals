namespace ChannelCatalog.UseCases.Channels.GetChannelById;

public sealed record GetChannelByIdResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string IntakeMode,
    string OwnershipStatus
);
