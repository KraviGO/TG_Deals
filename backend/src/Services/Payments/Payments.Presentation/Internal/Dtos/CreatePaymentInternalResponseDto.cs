namespace Payments.Presentation.Internal.Dtos;

public sealed record CreatePaymentInternalResponseDto(
    Guid PaymentId,
    string ConfirmationUrl
);
