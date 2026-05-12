using Publishers.Entities.Channels;

namespace Publishers.UseCases.Channels.CreateChannel;

public sealed record CreateChannelResult(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string Topic,
    string Language,
    decimal PricePerPostRub,
    IntakeMode IntakeMode,
    OwnershipStatus OwnershipStatus);
