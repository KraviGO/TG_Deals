using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Payments.Presentation.InternalWallet.Dtos;
using Payments.UseCases.Wallet.InternalCreditWallet;
using Payments.UseCases.Wallet.CaptureReservation;
using Payments.UseCases.Wallet.ReleaseReservation;
using Payments.UseCases.Wallet.ReserveForDeal;

namespace Payments.Presentation.InternalWallet;

[ApiController]
[Route("api/v1/internal/wallet")]
[Authorize(AuthenticationSchemes = "ServiceToken")]
public sealed class InternalWalletController : ControllerBase
{
    [HttpPost("dev/credit")]
    public async Task<ActionResult<InternalCreditWalletResponseDto>> DevCredit(
        [FromBody] InternalCreditWalletRequestDto dto,
        [FromServices] IWebHostEnvironment env,
        [FromServices] InternalCreditWalletHandler handler,
        CancellationToken ct)
    {
        if (!string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            return NotFound();

        var res = await handler.Handle(new InternalCreditWalletCommand(dto.UserId, dto.Amount, dto.Currency), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        var value = res.Value!;
        return Ok(new InternalCreditWalletResponseDto(value.Currency, value.Available, value.Reserved, value.Total));
    }

    [HttpPost("reservations")]
    public async Task<ActionResult<ReserveResponseDto>> Reserve(
        [FromBody] ReserveRequestDto dto,
        [FromServices] ReserveForDealHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new ReserveForDealCommand(dto.DealId, dto.AdvertiserUserId, dto.PublisherUserId, dto.Amount, dto.Currency), ct);
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
