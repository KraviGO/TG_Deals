namespace Payments.Presentation.Wallet.Dtos;

public sealed record WithdrawFromWalletRequestDto(
    decimal Amount,
    string Currency
);
