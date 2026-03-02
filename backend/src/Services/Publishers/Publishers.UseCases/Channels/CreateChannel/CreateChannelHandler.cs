using System.Text.Json;
using Marketplace.Contracts.Publishers;
using Publishers.Entities.Channels;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.CreateChannel;

public sealed class CreateChannelHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;

    public CreateChannelHandler(IPublishersDbContext db, IClock clock, IOutboxWriter outbox)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
    }

    public async Task<Result<CreateChannelResult>> Handle(CreateChannelCommand cmd, CancellationToken ct)
    {
        if (cmd.PublisherUserId == Guid.Empty) return Result<CreateChannelResult>.Fail(Errors.Validation);
        if (string.IsNullOrWhiteSpace(cmd.TelegramChannelId)) return Result<CreateChannelResult>.Fail(Errors.Validation);
        if (string.IsNullOrWhiteSpace(cmd.Title)) return Result<CreateChannelResult>.Fail(Errors.Validation);

        var now = _clock.UtcNow;
        var channel = Channel.Create(cmd.PublisherUserId, cmd.TelegramChannelId, new ChannelTitle(cmd.Title), now);

        await _db.AddChannelAsync(channel, ct);
        var evt = new ChannelRegisteredV1(
            ChannelId: channel.ChannelId.Value,
            PublisherUserId: channel.PublisherUserId,
            TelegramChannelId: channel.TelegramChannelId,
            Title: channel.Title,
            IntakeMode: channel.IntakeMode.ToString(),
            OwnershipStatus: channel.OwnershipStatus.ToString(),
            OccurredAtUtc: now
        );

        var payload = JsonSerializer.Serialize(evt);

        await _outbox.EnqueueAsync(new OutboxEnvelope(
            EventType: "ChannelRegistered",
            Version: 1,
            Exchange: "marketplace.events",
            RoutingKey: "publishers.channel.registered",
            PayloadJson: payload,
            OccurredAtUtc: now
        ), ct);
        await _db.SaveChangesAsync(ct);

        return Result<CreateChannelResult>.Ok(new CreateChannelResult(
            channel.ChannelId.Value,
            channel.TelegramChannelId,
            channel.Title,
            channel.IntakeMode,
            channel.OwnershipStatus
        ));
    }
}
