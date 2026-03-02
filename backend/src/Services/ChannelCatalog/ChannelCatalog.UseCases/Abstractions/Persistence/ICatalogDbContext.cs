using ChannelCatalog.Entities.Channels;

namespace ChannelCatalog.UseCases.Abstractions.Persistence;

public interface ICatalogDbContext
{
    IQueryable<CatalogChannel> CatalogChannels { get; }

    Task AddAsync(CatalogChannel entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
