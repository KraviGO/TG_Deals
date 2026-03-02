using ChannelCatalog.Presentation.Catalog.Dtos;
using ChannelCatalog.UseCases.Channels.GetChannelById;
using ChannelCatalog.UseCases.Channels.SearchChannels;
using Microsoft.AspNetCore.Mvc;

namespace ChannelCatalog.Presentation.Catalog;

[ApiController]
[Route("api/v1/catalog/channels")]
public sealed class CatalogController : ControllerBase
{
    private readonly SearchChannelsHandler _search;
    private readonly GetChannelByIdHandler _getById;

    public CatalogController(SearchChannelsHandler search, GetChannelByIdHandler getById)
    {
        _search = search;
        _getById = getById;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CatalogChannelResponseDto>>> Search([FromQuery] int limit = 50, [FromQuery] int offset = 0, CancellationToken ct = default)
    {
        var items = await _search.Handle(new SearchChannelsQuery(limit, offset), ct);
        return Ok(items.Select(x => new CatalogChannelResponseDto(x.ChannelId, x.TelegramChannelId, x.Title, x.IntakeMode, x.OwnershipStatus)).ToList());
    }

    [HttpGet("{channelId:guid}")]
    public async Task<ActionResult<CatalogChannelResponseDto>> GetById([FromRoute] Guid channelId, CancellationToken ct)
    {
        var item = await _getById.Handle(new GetChannelByIdQuery(channelId), ct);
        if (item is null) return NotFound();

        return Ok(new CatalogChannelResponseDto(item.ChannelId, item.TelegramChannelId, item.Title, item.IntakeMode, item.OwnershipStatus));
    }
}
