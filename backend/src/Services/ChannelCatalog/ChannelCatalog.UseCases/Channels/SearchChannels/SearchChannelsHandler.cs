using ChannelCatalog.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ChannelCatalog.UseCases.Channels.SearchChannels;

public sealed class SearchChannelsHandler
{
    private readonly ICatalogDbContext _db;

    public SearchChannelsHandler(ICatalogDbContext db) => _db = db;

    public async Task<IReadOnlyList<SearchChannelsResult>> Handle(SearchChannelsQuery q, CancellationToken ct)
    {
        var limit = Math.Clamp(q.Limit, 1, 200);
        var offset = Math.Max(0, q.Offset);

        var query = _db.CatalogChannels
            .Where(x => x.OwnershipStatus == "Verified");

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var search = q.Search.Trim();
            query = query.Where(x => x.Title.Contains(search) || x.TelegramChannelId.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(q.Topic))
        {
            var topic = q.Topic.Trim();
            query = query.Where(x => x.Topic == topic);
        }

        if (!string.IsNullOrWhiteSpace(q.Language))
        {
            var language = q.Language.Trim().ToLower(CultureInfo.InvariantCulture);
            query = query.Where(x => x.Language == language);
        }

        if (!string.IsNullOrWhiteSpace(q.IntakeMode))
        {
            var intakeMode = q.IntakeMode.Trim();
            query = query.Where(x => x.IntakeMode == intakeMode);
        }

        if (q.MinPricePerPostRub.HasValue)
            query = query.Where(x => x.PricePerPostRub >= q.MinPricePerPostRub.Value);

        if (q.MaxPricePerPostRub.HasValue)
            query = query.Where(x => x.PricePerPostRub <= q.MaxPricePerPostRub.Value);

        var sortBy = q.SortBy?.Trim().ToLowerInvariant();
        var desc = string.Equals(q.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy switch
        {
            "price" => desc
                ? query.OrderByDescending(x => x.PricePerPostRub).ThenBy(x => x.Title)
                : query.OrderBy(x => x.PricePerPostRub).ThenBy(x => x.Title),
            "title" => desc
                ? query.OrderByDescending(x => x.Title)
                : query.OrderBy(x => x.Title),
            _ => desc
                ? query.OrderByDescending(x => x.UpdatedAt)
                : query.OrderBy(x => x.UpdatedAt)
        };

        return await query
            .Skip(offset)
            .Take(limit)
            .Select(x => new SearchChannelsResult(
                x.ChannelId,
                x.TelegramChannelId,
                x.Title,
                x.Topic,
                x.Language,
                x.PricePerPostRub,
                x.IntakeMode,
                x.OwnershipStatus))
            .ToListAsync(ct);
    }
}
