namespace Payments.Presentation.Wallet.Dtos;

public sealed record WithdrawFromWalletResponseDto(
    string Currency,
    decimal Available,
    decimal Reserved,
    decimal Total
);
