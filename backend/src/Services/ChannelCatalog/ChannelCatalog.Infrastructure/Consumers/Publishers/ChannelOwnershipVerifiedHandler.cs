using System.Text.Json;
using ChannelCatalog.Infrastructure.Persistence;
using Marketplace.Contracts.Publishers;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.Infrastructure.Consumers.Publishers;

public sealed class ChannelOwnershipVerifiedHandler : IEventHandler
{
    public string RoutingKey => "publishers.channel.ownership_verified";

    private readonly CatalogDbContext _db;

    public ChannelOwnershipVerifiedHandler(CatalogDbContext db) => _db = db;

    public async Task HandleAsync(string payloadJson, CancellationToken ct)
    {
        var evt = JsonSerializer.Deserialize<ChannelOwnershipVerifiedV1>(payloadJson)
                  ?? throw new InvalidOperationException("Cannot deserialize ChannelOwnershipVerifiedV1.");

        var existing = await _db.CatalogChannelsSet.FirstOrDefaultAsync(x => x.ChannelId == evt.ChannelId, ct);
        if (existing is null) return;

        existing.MarkVerified(evt.OccurredAtUtc);

        await _db.SaveChangesAsync(ct);
    }
}
