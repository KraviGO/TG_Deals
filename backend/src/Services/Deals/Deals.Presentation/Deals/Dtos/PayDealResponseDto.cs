namespace Deals.Presentation.Deals.Dtos;

public sealed record PayDealResponseDto(Guid PaymentId, string ConfirmationUrl);
