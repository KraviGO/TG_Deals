using System.Text.Json;
using ChannelCatalog.Infrastructure.Persistence;
using Marketplace.Contracts.Publishers;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.Infrastructure.Consumers.Publishers;

public sealed class ChannelModerationChangedHandler : IEventHandler
{
    public string RoutingKey => "publishers.channel.moderation_changed.v1";

    private readonly CatalogDbContext _db;

    public ChannelModerationChangedHandler(CatalogDbContext db) => _db = db;

    public async Task HandleAsync(string payloadJson, CancellationToken ct)
    {
        var evt = JsonSerializer.Deserialize<ChannelModerationChangedV1>(payloadJson)
                  ?? throw new InvalidOperationException("Cannot deserialize ChannelModerationChangedV1.");

        var existing = await _db.CatalogChannelsSet.FirstOrDefaultAsync(x => x.ChannelId == evt.ChannelId, ct);
        if (existing is null) return;

        existing.SetOwnershipStatus(evt.OwnershipStatus, evt.OccurredAtUtc);

        await _db.SaveChangesAsync(ct);
    }
}
