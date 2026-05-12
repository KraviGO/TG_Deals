namespace Publishers.UseCases.Abstractions.Telegram;

public interface ITelegramChannelAccessClient
{
    Task<ChannelBotAccess> CheckAccessAsync(string telegramChannelId, CancellationToken ct);
}
