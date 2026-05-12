namespace Payments.Presentation.PublisherWallet.Dtos;

public sealed record WithdrawPublisherBalanceRequestDto(
    decimal? Amount,
    string? CardNumber,
    IReadOnlyList<Guid>? EntryIds
);
