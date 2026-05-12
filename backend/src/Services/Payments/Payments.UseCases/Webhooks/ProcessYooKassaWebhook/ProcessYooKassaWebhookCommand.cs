using Payments.UseCases.Abstractions.YooKassa;

namespace Payments.UseCases.Webhooks.ProcessYooKassaWebhook;

public sealed record ProcessYooKassaWebhookCommand(
    YooKassaWebhookNotification Notification,
    string? MessageId,
    string? RemoteIp
);
