namespace Payments.Presentation.Internal.Dtos;

public sealed record CreatePaymentInternalRequestDto(
    Guid DealId,
    Guid AdvertiserUserId,
    decimal Amount,
    string Currency
);
