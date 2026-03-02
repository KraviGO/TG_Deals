using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Persistence;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.Verification.StartVerification;

public sealed class StartVerificationHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;

    public StartVerificationHandler(IPublishersDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<StartVerificationResult>> Handle(StartVerificationCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result<StartVerificationResult>.Fail(Errors.NotFound);

        var code = $"VERIFY-{Guid.NewGuid():N}".ToUpperInvariant()[..14];
        var expiresAt = _clock.UtcNow.AddMinutes(30);

        var instruction = channel.StartVerification(code, expiresAt, _clock.UtcNow);
        await _db.SaveChangesAsync(ct);

        return Result<StartVerificationResult>.Ok(new StartVerificationResult(instruction, expiresAt));
    }
}
