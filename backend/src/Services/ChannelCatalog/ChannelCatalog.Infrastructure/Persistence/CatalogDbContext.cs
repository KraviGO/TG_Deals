using ChannelCatalog.Entities.Channels;
using ChannelCatalog.Infrastructure.Inbox;
using ChannelCatalog.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : DbContext, ICatalogDbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<CatalogChannel> CatalogChannelsSet => Set<CatalogChannel>();
    public IQueryable<CatalogChannel> CatalogChannels => CatalogChannelsSet.AsNoTracking();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public Task AddAsync(CatalogChannel entity, CancellationToken ct) => CatalogChannelsSet.AddAsync(entity, ct).AsTask();
    Task<int> ICatalogDbContext.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
