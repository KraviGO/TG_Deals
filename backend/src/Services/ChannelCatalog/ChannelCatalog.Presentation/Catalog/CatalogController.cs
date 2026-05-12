using ChannelCatalog.Presentation.Catalog.Dtos;
using ChannelCatalog.UseCases.Channels.GetChannelById;
using ChannelCatalog.UseCases.Channels.SearchChannels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChannelCatalog.Presentation.Catalog;

[ApiController]
[Route("api/v1/catalog/channels")]
[AllowAnonymous]
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
    public async Task<ActionResult<IReadOnlyList<CatalogChannelResponseDto>>> Search(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null,
        [FromQuery] string? topic = null,
        [FromQuery] string? language = null,
        [FromQuery] string? intakeMode = null,
        [FromQuery] decimal? minPricePerPostRub = null,
        [FromQuery] decimal? maxPricePerPostRub = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        CancellationToken ct = default)
    {
        var items = await _search.Handle(new SearchChannelsQuery(
            Limit: limit,
            Offset: offset,
            Search: search,
            Topic: topic,
            Language: language,
            IntakeMode: intakeMode,
            MinPricePerPostRub: minPricePerPostRub,
            MaxPricePerPostRub: maxPricePerPostRub,
            SortBy: sortBy,
            SortOrder: sortOrder), ct);

        return Ok(items.Select(x => new CatalogChannelResponseDto(
            x.ChannelId,
            x.TelegramChannelId,
            x.Title,
            x.Topic,
            x.Language,
            x.PricePerPostRub,
            x.IntakeMode,
            x.OwnershipStatus)).ToList());
    }

    [HttpGet("{channelId:guid}")]
    public async Task<ActionResult<CatalogChannelResponseDto>> GetById([FromRoute] Guid channelId, CancellationToken ct)
    {
        var item = await _getById.Handle(new GetChannelByIdQuery(channelId), ct);
        if (item is null) return NotFound();

        return Ok(new CatalogChannelResponseDto(
            item.ChannelId,
            item.TelegramChannelId,
            item.Title,
            item.Topic,
            item.Language,
            item.PricePerPostRub,
            item.IntakeMode,
            item.OwnershipStatus));
    }
}
