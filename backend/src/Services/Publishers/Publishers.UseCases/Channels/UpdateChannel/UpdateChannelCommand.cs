namespace Publishers.UseCases.Channels.UpdateChannel;

public sealed record UpdateChannelCommand(Guid PublisherUserId, Guid ChannelId, string TelegramChannelId, string Title);
