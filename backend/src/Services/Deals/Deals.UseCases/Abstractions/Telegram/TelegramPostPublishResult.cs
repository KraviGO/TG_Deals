namespace Deals.UseCases.Abstractions.Telegram;

public sealed record TelegramPostPublishResult(
    int MessageId,
    DateTimeOffset PostedAtUtc,
    string? PostUrl);
