using Marketplace.Contracts.Publishers;
using Publishers.UseCases.Abstractions.Clock;
using Publishers.UseCases.Abstractions.Messaging;
using Publishers.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Publishers.UseCases.Abstractions.Telegram;
using Publishers.UseCases.Common;

namespace Publishers.UseCases.Channels.Verification.ConfirmVerification;

/// <summary>
/// Подтверждает канал проверкой прав Telegram-бота.
/// </summary>
public sealed class ConfirmVerificationHandler
{
    private readonly IPublishersDbContext _db;
    private readonly IClock _clock;
    private readonly IOutboxWriter _outbox;
    private readonly ITelegramChannelAccessClient _telegram;

    public ConfirmVerificationHandler(
        IPublishersDbContext db,
        IClock clock,
        IOutboxWriter outbox,
        ITelegramChannelAccessClient telegram)
    {
        _db = db;
        _clock = clock;
        _outbox = outbox;
        _telegram = telegram;
    }

    public async Task<Result> Handle(ConfirmVerificationCommand cmd, CancellationToken ct)
    {
        var channel = await _db.FindChannelForOwnerAsync(cmd.PublisherUserId, cmd.ChannelId, ct);
        if (channel is null) return Result.Fail(Errors.NotFound);

        var now = _clock.UtcNow;

        ChannelBotAccess access;
        try
        {
            // Telegram-publisher проверяет права бота в канале.
            access = await _telegram.CheckAccessAsync(channel.TelegramChannelId, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Fail(Errors.TelegramPublisherUnavailable);
        }

        if (!access.HasRequiredPermissions)
            return Result.Fail(Errors.TelegramBotMissingChannelPermissions);

        try
        {
            // Повторное подтверждение не создает новое событие.
            var wasChanged = channel.VerifyOwnership(now);
            if (!wasChanged)
            {
                await _db.SaveChangesAsync(ct);
                return Result.Ok();
            }
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(Errors.InvalidState);
        }

        var evt = new ChannelOwnershipVerifiedV1(
            ChannelId: channel.ChannelId.Value,
            PublisherUserId: channel.PublisherUserId,
            OccurredAtUtc: now
        );

        // Catalog получает подтверждение владения через RabbitMQ.
        await _outbox.EnqueueAsync(
            exchange: "marketplace.events",
            routingKey: "publishers.channel.ownership_verified.v1",
            eventType: "ChannelOwnershipVerified",
            schemaVersion: 1,
            payload: evt,
            correlationId: channel.ChannelId.Value.ToString(),
            ct: ct
        );

        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
