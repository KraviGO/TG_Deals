using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.UseCases.Abstractions.YooKassa;
using Payments.UseCases.Webhooks.ProcessYooKassaWebhook;

namespace Payments.Presentation.Webhooks;

[ApiController]
[Route("api/v1/webhooks/yookassa")]
[AllowAnonymous]
public sealed class YooKassaWebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Receive(
        [FromBody] YooKassaWebhookNotification body,
        [FromServices] ProcessYooKassaWebhookHandler handler,
        CancellationToken ct)
    {
        var messageId = Request.Headers.TryGetValue("X-Request-Id", out var requestId)
            ? requestId.ToString()
            : null;
        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var res = await handler.Handle(new ProcessYooKassaWebhookCommand(
            Notification: body,
            MessageId: messageId,
            RemoteIp: remoteIp), ct);

        if (!res.IsSuccess)
        {
            if (res.Error is "UnauthorizedWebhookIp" or "UnauthorizedWebhookStatus")
                return Unauthorized(new { error = res.Error });
            return BadRequest(new { error = res.Error });
        }

        return Ok();
    }
}
