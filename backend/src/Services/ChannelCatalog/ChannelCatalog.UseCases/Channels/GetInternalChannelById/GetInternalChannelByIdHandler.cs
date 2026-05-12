using ChannelCatalog.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.UseCases.Channels.GetInternalChannelById;

/// <summary>
/// Внутренний запрос возвращает канал без фильтра по статусу.
/// </summary>
public sealed class GetInternalChannelByIdHandler
{
    private readonly ICatalogDbContext _db;

    public GetInternalChannelByIdHandler(ICatalogDbContext db) => _db = db;

    public async Task<GetInternalChannelByIdResult?> Handle(GetInternalChannelByIdQuery q, CancellationToken ct)
    {
        return await _db.CatalogChannels
            .Where(x => x.ChannelId == q.ChannelId)
            .Select(x => new GetInternalChannelByIdResult(
                x.ChannelId,
                x.PublisherUserId,
                x.TelegramChannelId,
                x.IntakeMode,
                x.OwnershipStatus))
            .FirstOrDefaultAsync(ct);
    }
}
