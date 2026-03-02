using Payments.UseCases.Abstractions.YooKassa;

namespace Payments.UseCases.Payments.ProcessWebhook;

public sealed record ProcessWebhookCommand(YooKassaWebhookNotification Notification);
