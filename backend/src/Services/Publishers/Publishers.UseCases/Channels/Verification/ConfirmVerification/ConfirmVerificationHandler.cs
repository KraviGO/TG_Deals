using System.Text.Json;
using Marketplace.Contracts.Publishers;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.Verification.ConfirmVerification;

public sealed class ConfirmVerificationHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;

    public ConfirmVerificationHandler(IPublishersDbContext db, IClock clock, IOutboxWriter outbox)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
    }

    public async Task<Result> Handle(ConfirmVerificationCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result.Fail(Errors.NotFound);

        var now = _clock.UtcNow;

        try
        {
            channel.ConfirmVerification(now);
        }
        catch (InvalidOperationException)
        {
            // Verification was not started or already expired
            return Result.Fail(Errors.Validation);
        }

        var evt = new ChannelOwnershipVerifiedV1(
            ChannelId: channel.ChannelId.Value,
            PublisherUserId: channel.PublisherUserId,
            OccurredAtUtc: now
        );

        await _outbox.EnqueueAsync(new OutboxEnvelope(
            EventType: "ChannelOwnershipVerified",
            Version: 1,
            Exchange: "marketplace.events",
            RoutingKey: "publishers.channel.ownership_verified",
            PayloadJson: JsonSerializer.Serialize(evt),
            OccurredAtUtc: now
        ), ct);

        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
