using ChannelCatalog.Presentation.Internal.Dtos;
using ChannelCatalog.UseCases.Abstractions.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChannelCatalog.Presentation.Internal;

[ApiController]
[Route("api/v1/internal/channels")]
public sealed class InternalChannelsController : ControllerBase
{
    private readonly ICatalogDbContext _db;

    public InternalChannelsController(ICatalogDbContext db) => _db = db;

    [HttpGet("{channelId:guid}")]
    public async Task<ActionResult<InternalChannelInfoDto>> Get([FromRoute] Guid channelId, CancellationToken ct)
    {
        var ch = await _db.CatalogChannels
            .Where(x => x.ChannelId == channelId)
            .Select(x => new InternalChannelInfoDto(x.ChannelId, x.PublisherUserId, x.IntakeMode, x.OwnershipStatus))
            .FirstOrDefaultAsync(ct);

        if (ch is null) return NotFound();
        return Ok(ch);
    }
}
