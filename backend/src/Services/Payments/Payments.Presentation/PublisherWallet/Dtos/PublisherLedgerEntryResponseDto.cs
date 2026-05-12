namespace Payments.Presentation.PublisherWallet.Dtos;

public sealed record PublisherLedgerEntryResponseDto(
    Guid EntryId,
    Guid DealId,
    decimal GrossAmount,
    decimal PlatformFeeAmount,
    decimal PublisherAmount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AvailableAt
);
