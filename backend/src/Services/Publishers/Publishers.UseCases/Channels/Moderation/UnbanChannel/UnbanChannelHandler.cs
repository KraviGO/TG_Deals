using System.Text.Json;
using Marketplace.Contracts.Publishers;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.Moderation.UnbanChannel;

/// <summary>
/// Обрабатывает запрос разбана канала.
/// </summary>
public sealed class UnbanChannelHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;

    public UnbanChannelHandler(IPublishersDbContext db, IClock clock, IOutboxWriter outbox)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
    }

    public async Task<Result> Handle(UnbanChannelCommand cmd, CancellationToken ct)
    {
        if (cmd.ChannelId == Guid.Empty) return Result.Fail(Errors.Validation);

        var ch = await _db.FindChannelAsync(cmd.ChannelId, ct);
        if (ch is null) return Result.Fail(Errors.NotFound);

        var now = _clock.UtcNow;

        try
        {
            // Домен запрещает разбан, бан является финальным.
            ch.UnbanToVerified(now);
        }
        catch
        {
            return Result.Fail(Errors.InvalidState);
        }

        var evt = new ChannelModerationChangedV1(
            ChannelId: ch.ChannelId.Value,
            PublisherUserId: ch.PublisherUserId,
            OwnershipStatus: ch.OwnershipStatus.ToString(),
            OccurredAtUtc: now);

        await _outbox.EnqueueAsync(
            exchange: "marketplace.events",
            routingKey: "publishers.channel.moderation_changed.v1",
            eventType: "ChannelModerationChanged",
            schemaVersion: 1,
            payload: evt,
            correlationId: ch.ChannelId.Value.ToString(),
            ct: ct
        );

        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
