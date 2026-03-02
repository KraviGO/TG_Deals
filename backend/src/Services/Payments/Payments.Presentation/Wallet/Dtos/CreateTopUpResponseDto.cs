namespace Payments.Presentation.Wallet.Dtos;

public sealed record CreateTopUpResponseDto(Guid TopUpId, string ConfirmationUrl);
