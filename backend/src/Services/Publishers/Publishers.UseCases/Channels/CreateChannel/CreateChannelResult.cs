using Publishers.Entities.Channels;

namespace Publishers.UseCases.Channels.CreateChannel;

public sealed record CreateChannelResult(Guid ChannelId, string TelegramChannelId, string Title, IntakeMode IntakeMode, OwnershipStatus OwnershipStatus);
