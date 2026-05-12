namespace ChannelCatalog.UseCases.Channels.GetInternalChannelById;

/// <summary>
/// Данные канала для внутренних вызовов.
/// </summary>
public sealed record GetInternalChannelByIdResult(
    Guid ChannelId,
    Guid PublisherUserId,
    string TelegramChannelId,
    string IntakeMode,
    string OwnershipStatus);
