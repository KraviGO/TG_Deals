using Payments.Entities.Common;

namespace Payments.Entities.Webhooks;

/// <summary>
/// Обработанный webhook YooKassa для дедупликации.
/// </summary>
public sealed class YooKassaWebhookInboxMessage : Entity
{
    private YooKassaWebhookInboxMessage() { }

    public string MessageId { get; private set; } = default!;
    public string EventType { get; private set; } = default!;
    public string YooKassaPaymentId { get; private set; } = default!;
    public string? RemoteIp { get; private set; }
    public DateTimeOffset ProcessedAt { get; private set; }

    /// <summary>
    /// Создает запись inbox после успешной обработки webhook.
    /// </summary>
    public static YooKassaWebhookInboxMessage Create(
        string messageId,
        string eventType,
        string yooKassaPaymentId,
        string? remoteIp,
        DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(messageId)) throw new ArgumentException("MessageId required");
        if (string.IsNullOrWhiteSpace(eventType)) throw new ArgumentException("EventType required");
        if (string.IsNullOrWhiteSpace(yooKassaPaymentId)) throw new ArgumentException("YooKassaPaymentId required");

        return new YooKassaWebhookInboxMessage
        {
            Id = Guid.NewGuid(),
            MessageId = messageId.Trim(),
            EventType = eventType.Trim(),
            YooKassaPaymentId = yooKassaPaymentId.Trim(),
            RemoteIp = string.IsNullOrWhiteSpace(remoteIp) ? null : remoteIp.Trim(),
            ProcessedAt = nowUtc
        };
    }
}
