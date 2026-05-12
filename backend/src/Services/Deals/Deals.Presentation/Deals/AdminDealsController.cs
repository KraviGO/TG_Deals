using System.Security.Claims;
using Deals.Presentation.Deals.Dtos;
using Deals.UseCases.Deals.Disputes.GetAdminDisputes;
using Deals.UseCases.Deals.Disputes.ResolveDispute;
using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deals.Presentation.Deals;

[ApiController]
[Route("api/v1/admin/deals")]
[Authorize(Policy = "Admin")]
public sealed class AdminDealsController : ControllerBase
{

    [HttpGet("disputes")]
    public async Task<ActionResult<IReadOnlyList<AdminDealDisputeResponseDto>>> GetDisputes(
        [FromQuery] string? status,
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromServices] GetAdminDisputesHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new GetAdminDisputesQuery(status, limit, offset), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x => new AdminDealDisputeResponseDto(
            x.DisputeId,
            x.DealId,
            x.DisputeStatus,
            x.Reason,
            x.OpenedByUserId,
            x.OpenedByRole,
            x.DisputeCreatedAt,
            x.ResolvedByUserId,
            x.ResolutionAction,
            x.ResolutionNote,
            x.ResolvedAt,
            x.ChannelId,
            x.AdvertiserUserId,
            x.PublisherUserId,
            x.DealStatus,
            x.FundingStatus,
            x.Amount,
            x.Currency,
            x.PostUrl,
            x.PublishedAtUtc
        )).ToList());
    }

    [HttpPost("{dealId:guid}/disputes/resolve")]
    public async Task<IActionResult> ResolveDispute(
        [FromRoute] Guid dealId,
        [FromBody] ResolveDisputeRequestDto dto,
        [FromServices] ResolveDealDisputeHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new ResolveDealDisputeCommand(
            AdminUserId: userId,
            DealId: dealId,
            Action: dto.Action,
            ResolutionNote: dto.ResolutionNote), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return NoContent();
    }
}
