namespace Payments.UseCases.Abstractions.YooKassa;

public sealed record YooKassaWebhookNotification(
    string Type,
    string Event,
    YooKassaPaymentObject Object
);

public sealed record YooKassaPaymentObject(
    string Id,
    string Status
);
