using System.Security.Claims;
using Deals.Presentation.Deals.Dtos;
using Deals.UseCases.Deals.CancelDeal;
using Deals.UseCases.Deals.CreateDeal;
using Deals.UseCases.Deals.DecideDeal;
using Deals.UseCases.Deals.GetMyDeals;
using Deals.UseCases.Deals.GetPublisherDeals;
using Deals.UseCases.Deals.PayDeal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deals.Presentation.Deals;

[ApiController]
[Route("api/v1/deals")]
public sealed class DealsController : ControllerBase
{
    private static Guid GetUserId(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    [HttpPost]
    [Authorize(Policy = "Advertiser")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateDealRequestDto dto,
        [FromServices] CreateDealHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);

        var res = await handler.Handle(new CreateDealCommand(
            AdvertiserUserId: userId,
            ChannelId: dto.ChannelId,
            PostText: dto.PostText,
            DesiredPublishAtUtc: dto.DesiredPublishAtUtc
        ), ct);

        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Created("", new { dealId = res.Value!.DealId, status = res.Value.Status });
    }

    [HttpGet("me")]
    [Authorize(Policy = "Advertiser")]
    public async Task<ActionResult<IReadOnlyList<DealResponseDto>>> MyDeals(
        [FromServices] GetMyDealsHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new GetMyDealsQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x =>
            new DealResponseDto(x.DealId, x.ChannelId, x.Status, x.DesiredPublishAtUtc, x.CreatedAt)
        ).ToList());
    }

    [HttpGet("publisher/inbox")]
    [Authorize(Policy = "Publisher")]
    public async Task<ActionResult<IReadOnlyList<object>>> PublisherInbox(
        [FromServices] GetPublisherDealsHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new GetPublisherDealsQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value);
    }

    [HttpPost("{dealId:guid}/publisher-decision")]
    [Authorize(Policy = "Publisher")]
    public async Task<IActionResult> PublisherDecision(
        [FromRoute] Guid dealId,
        [FromBody] PublisherDecisionRequestDto dto,
        [FromServices] PublisherDecideDealHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new PublisherDecideDealCommand(userId, dealId, dto.Accept), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return NoContent();
    }

    [HttpPost("{dealId:guid}/cancel")]
    [Authorize(Policy = "Advertiser")]
    public async Task<IActionResult> Cancel(
        [FromRoute] Guid dealId,
        [FromServices] CancelDealHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new CancelDealCommand(userId, dealId), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return NoContent();
    }

    [HttpPost("{dealId:guid}/pay")]
    [Authorize(Policy = "Advertiser")]
    public async Task<ActionResult<PayDealResponseDto>> Pay(
        [FromRoute] Guid dealId,
        [FromBody] PayDealRequestDto dto,
        [FromServices] PayDealHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new PayDealCommand(userId, dealId, dto.Amount, dto.Currency), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return Ok(new PayDealResponseDto(res.Value!.PaymentId, res.Value.ConfirmationUrl));
    }
}
