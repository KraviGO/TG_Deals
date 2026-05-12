namespace Deals.UseCases.Abstractions.Telegram;

public interface ITelegramPostPublisher
{
    Task<TelegramPostPublishResult> PublishTextAsync(string telegramChannelId, string text, CancellationToken ct);
}
