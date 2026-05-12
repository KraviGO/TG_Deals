using Deals.Presentation.Deals.Dtos;
using Deals.UseCases.Deals.AdvertiserConfirm;
using Deals.UseCases.Deals.CancelDeal;
using Deals.UseCases.Deals.ConfirmPublished;
using Deals.UseCases.Deals.CreateDeal;
using Deals.UseCases.Deals.DecideDeal;
using Deals.UseCases.Deals.Disputes.OpenDispute;
using Deals.UseCases.Deals.GetMyDeals;
using Deals.UseCases.Deals.GetPublisherDeals;
using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Deals.Presentation.Deals;

[ApiController]
[Route("api/v1/deals")]
public sealed class DealsController : ControllerBase
{

    [HttpPost]
    [Authorize(Policy = "Advertiser")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateDealRequestDto dto,
        [FromServices] CreateDealHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();

        var res = await handler.Handle(new CreateDealCommand(
            AdvertiserUserId: userId,
            ChannelId: dto.ChannelId,
            PostText: dto.PostText,
            DesiredPublishAtUtc: dto.DesiredPublishAtUtc,
            Amount: dto.Amount,
            Currency: dto.Currency
        ), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "TelegramPublishFailed" => StatusCode(StatusCodes.Status502BadGateway, new { error = res.Error }),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return Created("", new
        {
            dealId = res.Value!.DealId,
            status = res.Value.Status,
            fundingStatus = res.Value.FundingStatus,
            reservationId = res.Value.ReservationId
        });
    }

    [HttpGet("me")]
    [Authorize(Policy = "Advertiser")]
    public async Task<ActionResult<IReadOnlyList<DealResponseDto>>> MyDeals(
        [FromServices] GetMyDealsHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new GetMyDealsQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x =>
            new DealResponseDto(
                x.DealId,
                x.ChannelId,
                x.Status,
                x.FundingStatus,
                x.ReservationId,
                x.Amount,
                x.Currency,
                x.PostUrl,
                x.PublishedAtUtc,
                x.DesiredPublishAtUtc,
                x.CreatedAt)
        ).ToList());
    }

    [HttpGet("publisher/inbox")]
    [Authorize(Policy = "Publisher")]
    public async Task<ActionResult<IReadOnlyList<GetPublisherDealsResult>>> PublisherInbox(
        [FromServices] GetPublisherDealsHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
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
        var userId = User.GetUserId();
        var res = await handler.Handle(new PublisherDecideDealCommand(userId, dealId, dto.Accept), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                "TelegramPublishFailed" => StatusCode(StatusCodes.Status502BadGateway, new { error = res.Error }),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return NoContent();
    }

    [HttpPost("{dealId:guid}/confirm-published")]
    [Authorize(Policy = "Publisher")]
    public async Task<IActionResult> ConfirmPublished(
        [FromRoute] Guid dealId,
        [FromBody] ConfirmPublishedRequestDto dto,
        [FromServices] ConfirmPublishedHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new ConfirmPublishedCommand(
            PublisherUserId: userId,
            DealId: dealId,
            PostUrl: dto.PostUrl,
            PublishedAtUtc: dto.PublishedAtUtc,
            PublisherComment: dto.PublisherComment), ct);

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

    [HttpPost("{dealId:guid}/advertiser-confirm")]
    [Authorize(Policy = "Advertiser")]
    public async Task<IActionResult> AdvertiserConfirm(
        [FromRoute] Guid dealId,
        [FromServices] AdvertiserConfirmHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new AdvertiserConfirmCommand(
            AdvertiserUserId: userId,
            DealId: dealId), ct);

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
        var userId = User.GetUserId();
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

    [HttpPost("{dealId:guid}/disputes")]
    [Authorize]
    public async Task<ActionResult<object>> OpenDispute(
        [FromRoute] Guid dealId,
        [FromBody] OpenDisputeRequestDto dto,
        [FromServices] OpenDealDisputeHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var role = User.GetRole();

        var res = await handler.Handle(new OpenDealDisputeCommand(
            UserId: userId,
            UserRole: role,
            DealId: dealId,
            Reason: dto.Reason), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return Ok(new { disputeId = res.Value!.DisputeId, status = res.Value.Status });
    }

}
