namespace Publishers.UseCases.Channels.CreateChannel;

public sealed record CreateChannelCommand(Guid PublisherUserId, string TelegramChannelId, string Title);
