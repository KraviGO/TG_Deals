using System.Text.Json;
using Marketplace.Contracts.Publishers;
using Publishers.Entities.Channels;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Microsoft.EntityFrameworkCore;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.CreateChannel;

/// <summary>
/// Создает канал паблишера и событие для каталога.
/// </summary>
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

        string telegramChannelId;
        try
        {
            telegramChannelId = Channel.NormalizeTelegramChannelId(cmd.TelegramChannelId);
        }
        catch (ArgumentException)
        {
            return Result<CreateChannelResult>.Fail(Errors.Validation);
        }

        // TelegramChannelId уникален внутри каналов одного паблишера.
        var exists = await _db.Channels.AnyAsync(
            x => x.PublisherUserId == cmd.PublisherUserId
                 && (x.TelegramChannelId == telegramChannelId || x.TelegramChannelId == $"@{telegramChannelId}"),
            ct);
        if (exists) return Result<CreateChannelResult>.Fail(Errors.DuplicateChannel);

        var now = _clock.UtcNow;
        Channel channel;
        try
        {
            channel = Channel.Create(
                cmd.PublisherUserId,
                telegramChannelId,
                new ChannelTitle(cmd.Title),
                cmd.Topic,
                cmd.Language,
                cmd.PricePerPostRub,
                now);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Result<CreateChannelResult>.Fail(Errors.Validation);
        }

        await _db.AddChannelAsync(channel, ct);

        // Outbox публикует событие после сохранения транзакции.
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
            OccurredAtUtc: now
        );

        await _outbox.EnqueueAsync(
            exchange: "marketplace.events",
            routingKey: "publishers.channel.registered.v1",
            eventType: "ChannelRegistered",
            schemaVersion: 1,
            payload: evt,
            correlationId: channel.ChannelId.Value.ToString(),
            ct: ct
        );
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result<CreateChannelResult>.Fail(Errors.DuplicateChannel);
        }

        return Result<CreateChannelResult>.Ok(new CreateChannelResult(
            channel.ChannelId.Value,
            channel.TelegramChannelId,
            channel.Title,
            channel.Topic,
            channel.Language,
            channel.PricePerPostRub,
            channel.IntakeMode,
            channel.OwnershipStatus
        ));
    }
}
