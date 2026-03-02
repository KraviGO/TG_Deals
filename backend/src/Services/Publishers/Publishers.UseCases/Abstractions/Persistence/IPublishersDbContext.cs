using Publishers.Entities.Channels;

namespace Publishers.UseCases.Abstractions.Persistence;

public interface IPublishersDbContext
{
    IQueryable<Channel> Channels { get; }

    Task AddChannelAsync(Channel channel, CancellationToken ct);
    Task<Channel?> FindChannelForOwnerAsync(Guid publisherUserId, Guid channelId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
