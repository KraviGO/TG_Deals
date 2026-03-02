using Marketplace.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Publishers.Entities.Channels;
using Publishers.UseCases.Abstractions.Persistence;

namespace Publishers.Infrastructure.Persistence;

public sealed class PublishersDbContext : DbContext, IPublishersDbContext
{
    public PublishersDbContext(DbContextOptions<PublishersDbContext> options) : base(options) { }

    public DbSet<Channel> ChannelsSet => Set<Channel>();
    public IQueryable<Channel> Channels => ChannelsSet.AsQueryable();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public Task AddChannelAsync(Channel channel, CancellationToken ct) => ChannelsSet.AddAsync(channel, ct).AsTask();

    public Task<Channel?> FindChannelForOwnerAsync(Guid publisherUserId, Guid channelId, CancellationToken ct)
        => ChannelsSet.FirstOrDefaultAsync(
            c => c.PublisherUserId == publisherUserId && c.ChannelId == new ChannelId(channelId),
            ct);

    Task<int> IPublishersDbContext.SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PublishersDbContext).Assembly);
    }
}
