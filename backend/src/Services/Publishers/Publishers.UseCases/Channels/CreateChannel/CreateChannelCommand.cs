namespace Publishers.UseCases.Channels.CreateChannel;

public sealed record CreateChannelCommand(
    Guid PublisherUserId,
    string TelegramChannelId,
    string Title,
    string? Topic,
    string? Language,
    decimal? PricePerPostRub);
