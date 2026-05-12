using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Publishers.UseCases.Channels.Moderation.BanChannel;
using Publishers.UseCases.Channels.Moderation.UnbanChannel;

namespace Publishers.Presentation.Channels;

[ApiController]
[Route("api/v1/admin/publishers/channels")]
[Authorize(Policy = "Admin")]
public sealed class AdminChannelsController : ControllerBase
{
    [HttpPost("{channelId:guid}/ban")]
    public async Task<IActionResult> Ban(
        [FromRoute] Guid channelId,
        [FromServices] BanChannelHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new BanChannelCommand(channelId), ct);
        if (!res.IsSuccess)
            return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });

        return NoContent();
    }

    [HttpPost("{channelId:guid}/unban")]
    public async Task<IActionResult> Unban(
        [FromRoute] Guid channelId,
        [FromServices] UnbanChannelHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new UnbanChannelCommand(channelId), ct);
        if (!res.IsSuccess)
            return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });

        return NoContent();
    }
}
