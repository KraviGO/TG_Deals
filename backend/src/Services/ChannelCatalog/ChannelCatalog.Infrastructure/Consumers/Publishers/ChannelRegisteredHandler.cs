using System.Text.Json;
using ChannelCatalog.Entities.Channels;
using ChannelCatalog.Infrastructure.Persistence;
using Marketplace.Contracts.Publishers;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.Infrastructure.Consumers.Publishers;

public sealed class ChannelRegisteredHandler : IEventHandler
{
    public string RoutingKey => "publishers.channel.registered";

    private readonly CatalogDbContext _db;

    public ChannelRegisteredHandler(CatalogDbContext db) => _db = db;

    public async Task HandleAsync(string payloadJson, CancellationToken ct)
    {
        var evt = JsonSerializer.Deserialize<ChannelRegisteredV1>(payloadJson)
                  ?? throw new InvalidOperationException("Cannot deserialize ChannelRegisteredV1.");

        var existing = await _db.CatalogChannelsSet.FirstOrDefaultAsync(x => x.ChannelId == evt.ChannelId, ct);

        if (existing is null)
        {
            var entity = CatalogChannel.CreateFromRegistered(
                evt.ChannelId,
                evt.PublisherUserId,
                evt.TelegramChannelId,
                evt.Title,
                evt.IntakeMode,
                evt.OwnershipStatus,
                evt.OccurredAtUtc);

            await _db.CatalogChannelsSet.AddAsync(entity, ct);
        }
        else
        {
            existing.ApplyRegistered(
                evt.PublisherUserId,
                evt.TelegramChannelId,
                evt.Title,
                evt.IntakeMode,
                evt.OwnershipStatus,
                evt.OccurredAtUtc);
        }

        await _db.SaveChangesAsync(ct);
    }
}
