using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Presentation.Wallet.Dtos;
using Marketplace.Kernel.Results;
using Payments.UseCases.Common;
using Payments.UseCases.TopUps.GetMyTopUps;
using Payments.UseCases.TopUps.CreateTopUp;
using Payments.UseCases.Wallet.GetMyWallet;
using Payments.UseCases.Wallet.GetMyWalletTransactions;
using Payments.UseCases.Wallet.WithdrawFromWallet;

namespace Payments.Presentation.Wallet;

[ApiController]
[Route("api/v1/wallet/me")]
[Authorize(Policy = "Advertiser")]
public sealed class WalletController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<WalletDto>> GetMyWallet(
        [FromServices] GetMyWalletHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
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
        var userId = User.GetUserId();
        var res = await handler.Handle(new CreateTopUpCommand(userId, dto.Amount, dto.Currency), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(new CreateTopUpResponseDto(res.Value!.TopUpId, res.Value!.ConfirmationUrl));
    }

    [HttpGet("topups")]
    public async Task<ActionResult<IReadOnlyList<TopUpHistoryResponseDto>>> GetTopUps(
        [FromServices] GetMyTopUpsHandler handler,
        [FromQuery] int limit,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new GetMyTopUpsQuery(userId, limit), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x => new TopUpHistoryResponseDto(
            x.TopUpId,
            x.YooKassaPaymentId,
            x.Amount,
            x.Currency,
            x.Status,
            x.CreatedAt,
            x.UpdatedAt)).ToList());
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<IReadOnlyList<WalletTransactionResponseDto>>> GetTransactions(
        [FromServices] GetMyWalletTransactionsHandler handler,
        [FromQuery] int limit,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new GetMyWalletTransactionsQuery(userId, limit), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x => new WalletTransactionResponseDto(
            x.TxId,
            x.Type,
            x.Amount,
            x.Currency,
            x.DealId,
            x.TopUpId,
            x.CreatedAt)).ToList());
    }

    [HttpPost("withdrawals")]
    public async Task<ActionResult<WithdrawFromWalletResponseDto>> Withdraw(
        [FromBody] WithdrawFromWalletRequestDto dto,
        [FromServices] WithdrawFromWalletHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new WithdrawFromWalletCommand(userId, dto.Amount, dto.Currency), ct);
        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                Errors.NotFound => NotFound(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        var v = res.Value!;
        return Ok(new WithdrawFromWalletResponseDto(v.Currency, v.Available, v.Reserved, v.Total));
    }
}
