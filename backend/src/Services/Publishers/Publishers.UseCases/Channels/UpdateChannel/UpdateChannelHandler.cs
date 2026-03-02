using Publishers.Entities.Channels;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Persistence;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.UpdateChannel;

public sealed class UpdateChannelHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;

    public UpdateChannelHandler(IPublishersDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result> Handle(UpdateChannelCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result.Fail(Errors.NotFound);

        channel.Update(cmd.TelegramChannelId, new ChannelTitle(cmd.Title), _clock.UtcNow);
        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
