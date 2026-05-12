namespace Payments.Presentation.InternalWallet.Dtos;

public sealed record InternalCreditWalletResponseDto(
    string Currency,
    decimal Available,
    decimal Reserved,
    decimal Total
);
