namespace Payments.Presentation.PublisherWallet.Dtos;

public sealed record WithdrawPublisherBalanceResponseDto(
    string Currency,
    decimal RequestedAmount,
    decimal WithdrawnAmount,
    decimal Available,
    decimal PaidOut,
    string? DestinationCardMask
);
