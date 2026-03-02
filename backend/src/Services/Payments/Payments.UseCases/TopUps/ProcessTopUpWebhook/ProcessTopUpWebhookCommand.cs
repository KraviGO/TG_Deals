using Payments.UseCases.Abstractions.YooKassa;

namespace Payments.UseCases.TopUps.ProcessTopUpWebhook;

public sealed record ProcessTopUpWebhookCommand(YooKassaWebhookNotification Notification);
