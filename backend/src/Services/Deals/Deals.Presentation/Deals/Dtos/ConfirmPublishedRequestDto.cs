namespace Deals.Presentation.Deals.Dtos;

public sealed record ConfirmPublishedRequestDto(
    string? PostUrl,
    DateTimeOffset? PublishedAtUtc,
    string? PublisherComment
);
