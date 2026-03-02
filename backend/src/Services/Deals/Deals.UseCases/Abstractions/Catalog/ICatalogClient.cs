namespace Deals.UseCases.Abstractions.Catalog;

public interface ICatalogClient
{
    Task<CatalogChannelInfo?> GetChannelAsync(Guid channelId, CancellationToken ct);
}
