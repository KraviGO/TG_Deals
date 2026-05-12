namespace Payments.Presentation.InternalWallet.Dtos;

public sealed record InternalCreditWalletRequestDto(
    Guid UserId,
    decimal Amount,
    string Currency
);
