using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payments.Entities.Webhooks;
using Payments.UseCases.Abstractions.Clock;
using Payments.UseCases.Abstractions.Persistence;
using Payments.UseCases.Abstractions.YooKassa;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;

using Payments.UseCases.TopUps.ProcessTopUpWebhook;
using System.Net;

namespace Payments.UseCases.Webhooks.ProcessYooKassaWebhook;

/// <summary>
/// Проверяет webhook YooKassa и передает его обработчику пополнений.
/// </summary>
public sealed class ProcessYooKassaWebhookHandler
{
    private readonly IPaymentsDbContext _db;
    private readonly IYooKassaClient _yoo;
    private readonly IClock _clock;
    private readonly YooKassaWebhookSecurityOptions _security;
    private readonly ProcessTopUpWebhookHandler _topUpHandler;

    public ProcessYooKassaWebhookHandler(
        IPaymentsDbContext db,
        IYooKassaClient yoo,
        IClock clock,
        IOptions<YooKassaWebhookSecurityOptions> security,
        ProcessTopUpWebhookHandler topUpHandler)
    {
        _db = db;
        _yoo = yoo;
        _clock = clock;
        _security = security.Value;
        _topUpHandler = topUpHandler;
    }

    public async Task<Result> Handle(ProcessYooKassaWebhookCommand cmd, CancellationToken ct)
    {
        var dedupId = BuildDedupId(cmd.MessageId, cmd.Notification);

        // Inbox блокирует повторную доставку webhook.
        var alreadyProcessed = await _db.YooKassaWebhookInboxMessages.AnyAsync(x => x.MessageId == dedupId, ct);
        if (alreadyProcessed)
            return Result.Ok();

        // Allowlist принимает IP YooKassa или доверенного reverse proxy.
        if (!IsIpAllowed(cmd.RemoteIp, _security.AllowedIps))
            return Result.Fail("UnauthorizedWebhookIp");

        if (_security.RequireStatusVerification)
        {
            // Verification сверяет webhook со статусом платежа в YooKassa.
            var actualStatus = await _yoo.GetPaymentStatusAsync(cmd.Notification.Object.Id, ct);
            if (string.IsNullOrWhiteSpace(actualStatus) ||
                !string.Equals(actualStatus, cmd.Notification.Object.Status, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail("UnauthorizedWebhookStatus");
            }
        }

        var topupRes = await _topUpHandler.Handle(new ProcessTopUpWebhookCommand(cmd.Notification), ct);
        if (!topupRes.IsSuccess) return Result.Fail(topupRes.Error ?? Errors.Validation);

        var inbox = YooKassaWebhookInboxMessage.Create(
            messageId: dedupId,
            eventType: cmd.Notification.Event,
            yooKassaPaymentId: cmd.Notification.Object.Id,
            remoteIp: cmd.RemoteIp,
            nowUtc: _clock.UtcNow);

        await _db.AddYooKassaWebhookInboxMessageAsync(inbox, ct);
        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }

    private static string BuildDedupId(string? providedMessageId, YooKassaWebhookNotification n)
    {
        if (!string.IsNullOrWhiteSpace(providedMessageId))
            return providedMessageId.Trim();

        // Без X-Request-Id dedup key строится из события и payment id.
        return $"{n.Event}:{n.Object.Id}";
    }

    private static bool IsIpAllowed(string? remoteIp, IReadOnlyList<string>? allowlist)
    {
        var normalizedAllowlist = (allowlist ?? Array.Empty<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedAllowlist.Length == 0)
            return true;

        if (string.IsNullOrWhiteSpace(remoteIp))
            return false;

        if (!IPAddress.TryParse(remoteIp, out var ip))
            return false;

        foreach (var candidate in normalizedAllowlist)
        {
            if (!candidate.Contains('/'))
            {
                if (IPAddress.TryParse(candidate, out var single) && single.Equals(ip))
                    return true;
                continue;
            }

            if (IpInCidr(ip, candidate))
                return true;
        }

        return false;
    }

    private static bool IpInCidr(IPAddress ip, string cidr)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2) return false;

        if (!IPAddress.TryParse(parts[0], out var networkIp)) return false;
        if (!int.TryParse(parts[1], out var prefixLength)) return false;

        var ipBytes = ip.GetAddressBytes();
        var netBytes = networkIp.GetAddressBytes();
        if (ipBytes.Length != netBytes.Length) return false;

        var totalBits = ipBytes.Length * 8;
        if (prefixLength < 0 || prefixLength > totalBits) return false;

        var fullBytes = prefixLength / 8;
        var remainingBits = prefixLength % 8;

        for (var i = 0; i < fullBytes; i++)
        {
            if (ipBytes[i] != netBytes[i])
                return false;
        }

        if (remainingBits == 0)
            return true;

        var mask = (byte)(0xFF << (8 - remainingBits));
        return (ipBytes[fullBytes] & mask) == (netBytes[fullBytes] & mask);
    }
}
