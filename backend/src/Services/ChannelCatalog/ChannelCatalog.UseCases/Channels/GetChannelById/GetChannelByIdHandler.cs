using ChannelCatalog.UseCases.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.UseCases.Channels.GetChannelById;

public sealed class GetChannelByIdHandler
{
    private readonly ICatalogDbContext _db;

    public GetChannelByIdHandler(ICatalogDbContext db) => _db = db;

    public async Task<GetChannelByIdResult?> Handle(GetChannelByIdQuery q, CancellationToken ct)
    {
        return await _db.CatalogChannels
            .Where(x => x.ChannelId == q.ChannelId && x.OwnershipStatus == "Verified")
            .Select(x => new GetChannelByIdResult(
                x.ChannelId,
                x.TelegramChannelId,
                x.Title,
                x.IntakeMode,
                x.OwnershipStatus))
            .FirstOrDefaultAsync(ct);
    }
}
