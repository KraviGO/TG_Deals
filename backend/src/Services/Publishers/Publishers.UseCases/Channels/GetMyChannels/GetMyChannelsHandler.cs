using Microsoft.EntityFrameworkCore;
using Publishers.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.GetMyChannels;

/// <summary>
/// Возвращает каналы текущего паблишера.
/// </summary>
public sealed class GetMyChannelsHandler
{
    private readonly IPublishersDbContext _db;

    public GetMyChannelsHandler(IPublishersDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<GetMyChannelsResult>>> Handle(GetMyChannelsQuery query, CancellationToken ct)
    {
        if (query.PublisherUserId == Guid.Empty)
            return Result<IReadOnlyList<GetMyChannelsResult>>.Fail(Errors.Validation);

        var items = await _db.Channels
            .Where(c => c.PublisherUserId == query.PublisherUserId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new GetMyChannelsResult(
                c.ChannelId.Value,
                c.TelegramChannelId,
                c.Title,
                c.Topic,
                c.Language,
                c.PricePerPostRub,
                c.IntakeMode,
                c.OwnershipStatus))
            .ToListAsync(ct);

        return Result<IReadOnlyList<GetMyChannelsResult>>.Ok(items);
    }
}
