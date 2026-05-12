using System.Text.Json;
using Marketplace.Contracts.Publishers;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.SetIntakeMode;

/// <summary>
/// Меняет режим приема сделок по каналу.
/// </summary>
public sealed class SetIntakeModeHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;

    public SetIntakeModeHandler(IPublishersDbContext db, IClock clock, IOutboxWriter outbox)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
    }

    public async Task<Result> Handle(SetIntakeModeCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result.Fail(Errors.NotFound);

        var now = _clock.UtcNow;
        try
        {
            channel.SetIntakeMode(cmd.Mode, now);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(Errors.InvalidState);
        }

        // Catalog получает новый intake mode через полный снимок канала.
        var evt = new ChannelRegisteredV1(
            ChannelId: channel.ChannelId.Value,
            PublisherUserId: channel.PublisherUserId,
            TelegramChannelId: channel.TelegramChannelId,
            Title: channel.Title,
            Topic: channel.Topic,
            Language: channel.Language,
            PricePerPostRub: channel.PricePerPostRub,
            IntakeMode: channel.IntakeMode.ToString(),
            OwnershipStatus: channel.OwnershipStatus.ToString(),
            OccurredAtUtc: now);

        await _outbox.EnqueueAsync(
            exchange: "marketplace.events",
            routingKey: "publishers.channel.registered.v1",
            eventType: "ChannelRegistered",
            schemaVersion: 1,
            payload: evt,
            correlationId: channel.ChannelId.Value.ToString(),
            ct: ct
        );

        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
