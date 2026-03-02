using Microsoft.AspNetCore.Mvc;
using Payments.Presentation.InternalWallet.Dtos;
using Payments.UseCases.Wallet.CaptureReservation;
using Payments.UseCases.Wallet.ReleaseReservation;
using Payments.UseCases.Wallet.ReserveForDeal;

namespace Payments.Presentation.InternalWallet;

[ApiController]
[Route("api/v1/internal/wallet")]
public sealed class InternalWalletController : ControllerBase
{
    [HttpPost("reservations")]
    public async Task<ActionResult<ReserveResponseDto>> Reserve(
        [FromBody] ReserveRequestDto dto,
        [FromServices] ReserveForDealHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new ReserveForDealCommand(dto.DealId, dto.AdvertiserUserId, dto.Amount, dto.Currency), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        return Ok(new ReserveResponseDto(res.Value!.ReservationId, res.Value!.Status));
    }

    [HttpPost("reservations/{dealId:guid}/release")]
    public async Task<IActionResult> Release(
        [FromRoute] Guid dealId,
        [FromServices] ReleaseReservationHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new ReleaseReservationCommand(dealId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        return NoContent();
    }

    [HttpPost("reservations/{dealId:guid}/capture")]
    public async Task<IActionResult> Capture(
        [FromRoute] Guid dealId,
        [FromServices] CaptureReservationHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new CaptureReservationCommand(dealId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        return NoContent();
    }
}
