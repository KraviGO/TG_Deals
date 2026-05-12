namespace Publishers.UseCases.Common;

public static class Errors
{
    public const string NotFound = "NotFound";
    public const string Forbidden = "Forbidden";
    public const string Validation = "Validation";
    public const string InvalidState = "InvalidState";
    public const string DuplicateChannel = "DuplicateChannel";
    public const string TelegramBotMissingChannelPermissions = "TelegramBotMissingChannelPermissions";
    public const string TelegramPublisherUnavailable = "TelegramPublisherUnavailable";
}
