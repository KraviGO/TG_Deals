using ChannelCatalog.Presentation.Internal.Dtos;
using ChannelCatalog.UseCases.Channels.GetInternalChannelById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChannelCatalog.Presentation.Internal;

[ApiController]
[Route("api/v1/internal/channels")]
[Authorize(AuthenticationSchemes = "ServiceToken")]
public sealed class InternalChannelsController : ControllerBase
{
    [HttpGet("{channelId:guid}")]
    public async Task<ActionResult<InternalChannelInfoDto>> Get(
        [FromRoute] Guid channelId,
        [FromServices] GetInternalChannelByIdHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new GetInternalChannelByIdQuery(channelId), ct);
        if (result is null) return NotFound();
        return Ok(new InternalChannelInfoDto(
            result.ChannelId,
            result.PublisherUserId,
            result.TelegramChannelId,
            result.IntakeMode,
            result.OwnershipStatus));
    }
}
