namespace Payments.Presentation.Wallet.Dtos;

public sealed record CreateTopUpRequestDto(decimal Amount, string Currency);
