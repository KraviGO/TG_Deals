using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Presentation.Wallet.Dtos;
using Payments.UseCases.TopUps.CreateTopUp;
using Payments.UseCases.Wallet.GetMyWallet;

namespace Payments.Presentation.Wallet;

[ApiController]
[Route("api/v1/wallet/me")]
[Authorize(Policy = "Advertiser")]
public sealed class WalletController : ControllerBase
{
    private static Guid GetUserId(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<ActionResult<WalletDto>> GetMyWallet(
        [FromServices] GetMyWalletHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new GetMyWalletQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        return Ok(res.Value);
    }

    [HttpPost("topups")]
    public async Task<ActionResult<CreateTopUpResponseDto>> CreateTopUp(
        [FromBody] CreateTopUpRequestDto dto,
        [FromServices] CreateTopUpHandler handler,
        CancellationToken ct)
    {
        var userId = GetUserId(User);
        var res = await handler.Handle(new CreateTopUpCommand(userId, dto.Amount, dto.Currency), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(new CreateTopUpResponseDto(res.Value!.TopUpId, res.Value!.ConfirmationUrl));
    }
}
