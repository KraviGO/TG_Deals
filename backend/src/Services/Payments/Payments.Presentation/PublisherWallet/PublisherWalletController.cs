using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Presentation.PublisherWallet.Dtos;
using Payments.UseCases.PublisherWallet.GetMyPublisherLedgerEntries;
using Payments.UseCases.PublisherWallet.GetMyPublisherWallet;
using Payments.UseCases.PublisherWallet.WithdrawPublisherBalance;

namespace Payments.Presentation.PublisherWallet;

[ApiController]
[Route("api/v1/publisher/wallet/me")]
[Authorize(Policy = "Publisher")]
public sealed class PublisherWalletController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<PublisherWalletResponseDto>> GetMyWallet(
        [FromServices] GetMyPublisherWalletHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new GetMyPublisherWalletQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        var dto = res.Value!;
        return Ok(new PublisherWalletResponseDto(dto.Currency, dto.Available, dto.PaidOut, dto.TotalAccrued));
    }

    [HttpGet("entries")]
    public async Task<ActionResult<IReadOnlyList<PublisherLedgerEntryResponseDto>>> GetMyEntries(
        [FromServices] GetMyPublisherLedgerEntriesHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new GetMyPublisherLedgerEntriesQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x => new PublisherLedgerEntryResponseDto(
            x.EntryId,
            x.DealId,
            x.GrossAmount,
            x.PlatformFeeAmount,
            x.PublisherAmount,
            x.Currency,
            x.Status,
            x.CreatedAt,
            x.AvailableAt)).ToList());
    }

    [HttpPost("withdrawals")]
    public async Task<ActionResult<WithdrawPublisherBalanceResponseDto>> Withdraw(
        [FromBody] WithdrawPublisherBalanceRequestDto dto,
        [FromServices] WithdrawPublisherBalanceHandler handler,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await handler.Handle(new WithdrawPublisherBalanceCommand(
            PublisherUserId: userId,
            EntryIds: dto.EntryIds,
            Amount: dto.Amount,
            CardNumber: dto.CardNumber), ct);

        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                _ => BadRequest(new { error = res.Error })
            };
        }

        var v = res.Value!;
        return Ok(new WithdrawPublisherBalanceResponseDto(
            v.Currency,
            v.RequestedAmount,
            v.WithdrawnAmount,
            v.Available,
            v.PaidOut,
            v.DestinationCardMask));
    }
}
