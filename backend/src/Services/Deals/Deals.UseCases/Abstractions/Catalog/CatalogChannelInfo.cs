namespace Deals.UseCases.Abstractions.Catalog;

public sealed record CatalogChannelInfo(
    Guid ChannelId,
    Guid PublisherUserId,
    string IntakeMode,
    string OwnershipStatus
);
