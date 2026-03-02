using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Persistence;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.SetIntakeMode;

public sealed class SetIntakeModeHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;

    public SetIntakeModeHandler(IPublishersDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result> Handle(SetIntakeModeCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result.Fail(Errors.NotFound);

        channel.SetIntakeMode(cmd.Mode, _clock.UtcNow);
        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
