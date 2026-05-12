namespace Publishers.UseCases.Abstractions.Telegram;

/// <summary>
/// Права Telegram-бота в канале.
/// </summary>
public sealed record ChannelBotAccess(
    bool IsAccessible,
    bool IsBotAdmin,
    bool CanPostMessages,
    bool CanDeleteMessages)
{
    public bool HasRequiredPermissions => IsAccessible && IsBotAdmin && CanPostMessages && CanDeleteMessages;
}
