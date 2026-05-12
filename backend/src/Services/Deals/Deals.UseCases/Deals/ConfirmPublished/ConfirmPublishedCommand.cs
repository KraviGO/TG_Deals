namespace Deals.UseCases.Deals.ConfirmPublished;

public sealed record ConfirmPublishedCommand(
    Guid PublisherUserId,
    Guid DealId,
    string? PostUrl,
    DateTimeOffset? PublishedAtUtc,
    string? PublisherComment
);
