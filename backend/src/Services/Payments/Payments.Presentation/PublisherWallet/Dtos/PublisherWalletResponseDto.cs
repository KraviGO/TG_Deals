namespace Payments.Presentation.PublisherWallet.Dtos;

public sealed record PublisherWalletResponseDto(
    string Currency,
    decimal Available,
    decimal PaidOut,
    decimal TotalAccrued
);
