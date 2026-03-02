using Microsoft.AspNetCore.Mvc;
using Payments.Presentation.Internal.Dtos;
using Payments.UseCases.Payments.CapturePayment;
using Payments.UseCases.Payments.CreatePayment;

namespace Payments.Presentation.Internal;

[ApiController]
[Route("api/v1/internal/payments")]
public sealed class InternalPaymentsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreatePaymentInternalResponseDto>> Create(
        [FromBody] CreatePaymentInternalRequestDto dto,
        [FromServices] CreatePaymentHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new CreatePaymentCommand(dto.DealId, dto.AdvertiserUserId, dto.Amount, dto.Currency), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(new CreatePaymentInternalResponseDto(res.Value!.PaymentId, res.Value!.ConfirmationUrl));
    }

    [HttpPost("{paymentId:guid}/capture")]
    public async Task<IActionResult> Capture(
        [FromRoute] Guid paymentId,
        [FromServices] CapturePaymentHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(new CapturePaymentCommand(paymentId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        return NoContent();
    }
}
