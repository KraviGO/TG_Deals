using ChannelCatalog.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.UseCases.Channels.SearchChannels;

public sealed class SearchChannelsHandler
{
    private readonly ICatalogDbContext _db;

    public SearchChannelsHandler(ICatalogDbContext db) => _db = db;

    public async Task<IReadOnlyList<SearchChannelsResult>> Handle(SearchChannelsQuery q, CancellationToken ct)
    {
        var limit = Math.Clamp(q.Limit, 1, 200);
        var offset = Math.Max(0, q.Offset);

        return await _db.CatalogChannels
            .Where(x => x.OwnershipStatus == "Verified")
            .OrderByDescending(x => x.UpdatedAt)
            .Skip(offset)
            .Take(limit)
            .Select(x => new SearchChannelsResult(
                x.ChannelId,
                x.TelegramChannelId,
                x.Title,
                x.IntakeMode,
                x.OwnershipStatus))
            .ToListAsync(ct);
    }
}
