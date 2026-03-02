using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Payments.UseCases.Abstractions.YooKassa;
using Payments.UseCases.TopUps.ProcessTopUpWebhook;
using Payments.UseCases.Payments.ProcessWebhook;

namespace Payments.Presentation.Webhooks;

[ApiController]
[Route("api/v1/webhooks/yookassa")]
[AllowAnonymous] // webhook должен быть доступен без JWT; защиту делает секрет в URL
public sealed class YooKassaWebhookController : ControllerBase
{
    private readonly string _secret;

    public YooKassaWebhookController(IConfiguration cfg)
    {
        _secret = cfg["Webhooks:YooKassa:Secret"] ?? throw new InvalidOperationException("Webhooks:YooKassa:Secret missing");
    }

    [HttpPost("{secret}")]
    public async Task<IActionResult> Receive(
        [FromRoute] string secret,
        [FromBody] YooKassaWebhookNotification body,
        [FromServices] ProcessTopUpWebhookHandler topupHandler,
        [FromServices] ProcessWebhookHandler legacyPaymentsHandler,
        CancellationToken ct)
    {
        if (!FixedTimeEquals(secret, _secret))
            return Unauthorized();

        // Сначала обрабатываем TopUp (wallet), затем legacy payments (пока не вычищены)
        var topupRes = await topupHandler.Handle(new ProcessTopUpWebhookCommand(body), ct);
        if (!topupRes.IsSuccess) return BadRequest(new { error = topupRes.Error });

        var res = await legacyPaymentsHandler.Handle(new ProcessWebhookCommand(body), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok();
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var diff = 0;
        for (var i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }
}
