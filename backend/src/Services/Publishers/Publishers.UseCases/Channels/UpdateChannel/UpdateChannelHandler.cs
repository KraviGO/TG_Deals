using System.Text.Json;
using Marketplace.Contracts.Publishers;
using Publishers.Entities.Channels;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.UpdateChannel;

/// <summary>
/// Обновляет профиль канала и событие для каталога.
/// </summary>
public sealed class UpdateChannelHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;

    public UpdateChannelHandler(IPublishersDbContext db, IClock clock, IOutboxWriter outbox)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
    }

    public async Task<Result> Handle(UpdateChannelCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result.Fail(Errors.NotFound);

        var now = _clock.UtcNow;
        try
        {
            channel.Update(
                cmd.TelegramChannelId,
                new ChannelTitle(cmd.Title),
                cmd.Topic,
                cmd.Language,
                cmd.PricePerPostRub,
                now);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Result.Fail(Errors.Validation);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(Errors.InvalidState);
        }

        // Catalog получает полный снимок канала тем же контрактом.
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
