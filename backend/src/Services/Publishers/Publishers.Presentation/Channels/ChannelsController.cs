using Marketplace.Security.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Publishers.Entities.Channels;
using Publishers.Presentation.Channels.Dtos;
using Publishers.UseCases.Channels.CreateChannel;
using Publishers.UseCases.Channels.GetMyChannels;
using Publishers.UseCases.Channels.SetIntakeMode;
using Publishers.UseCases.Channels.UpdateChannel;
using Publishers.UseCases.Channels.Verification.ConfirmVerification;

namespace Publishers.Presentation.Channels;

[ApiController]
[Route("api/v1/publishers/me/channels")]
[Authorize(Policy = "Publisher")]
public sealed class ChannelsController : ControllerBase
{
    private readonly CreateChannelHandler _create;
    private readonly GetMyChannelsHandler _getMy;
    private readonly UpdateChannelHandler _update;
    private readonly SetIntakeModeHandler _setMode;
    private readonly ConfirmVerificationHandler _confirmVerify;

    public ChannelsController(
        CreateChannelHandler create,
        GetMyChannelsHandler getMy,
        UpdateChannelHandler update,
        SetIntakeModeHandler setMode,
        ConfirmVerificationHandler confirmVerify)
    {
        _create = create;
        _getMy = getMy;
        _update = update;
        _setMode = setMode;
        _confirmVerify = confirmVerify;
    }


    [HttpPost]
    public async Task<ActionResult<ChannelResponseDto>> Create([FromBody] CreateChannelRequestDto dto, CancellationToken ct)
    {
        // Owner берется из JWT.
        var userId = User.GetUserId();
        var res = await _create.Handle(new CreateChannelCommand(
            userId,
            dto.TelegramChannelId,
            dto.Title,
            dto.Topic,
            dto.Language,
            dto.PricePerPostRub), ct);
        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "DuplicateChannel" => Conflict(new { error = res.Error }),
                _ => BadRequest(new { error = res.Error })
            };
        }

        var v = res.Value!;
        return Created("", new ChannelResponseDto(
            v.ChannelId,
            v.TelegramChannelId,
            v.Title,
            v.Topic,
            v.Language,
            v.PricePerPostRub,
            v.IntakeMode.ToString(),
            v.OwnershipStatus.ToString()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ChannelResponseDto>>> GetMy(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await _getMy.Handle(new GetMyChannelsQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x =>
            new ChannelResponseDto(
                x.ChannelId,
                x.TelegramChannelId,
                x.Title,
                x.Topic,
                x.Language,
                x.PricePerPostRub,
                x.IntakeMode.ToString(),
                x.OwnershipStatus.ToString())
        ).ToList());
    }

    [HttpPut("{channelId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid channelId, [FromBody] UpdateChannelRequestDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var res = await _update.Handle(new UpdateChannelCommand(
            userId,
            channelId,
            dto.TelegramChannelId,
            dto.Title,
            dto.Topic,
            dto.Language,
            dto.PricePerPostRub), ct);
        if (!res.IsSuccess) return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });
        return NoContent();
    }

    [HttpPatch("{channelId:guid}/intake-mode")]
    public async Task<IActionResult> SetIntakeMode([FromRoute] Guid channelId, [FromBody] SetIntakeModeRequestDto dto, CancellationToken ct)
    {
        if (!Enum.TryParse<IntakeMode>(dto.Mode, ignoreCase: true, out var mode))
            return BadRequest(new { error = "InvalidMode" });

        var userId = User.GetUserId();
        var res = await _setMode.Handle(new SetIntakeModeCommand(userId, channelId, mode), ct);
        if (!res.IsSuccess) return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });
        return NoContent();
    }

    [HttpPost("{channelId:guid}/verify/confirm")]
    public async Task<IActionResult> ConfirmVerification([FromRoute] Guid channelId, CancellationToken ct)
    {
        // Верификация проверяет права Telegram-бота.
        var userId = User.GetUserId();
        var res = await _confirmVerify.Handle(new ConfirmVerificationCommand(userId, channelId), ct);
        if (!res.IsSuccess)
        {
            return res.Error switch
            {
                "NotFound" => NotFound(),
                "TelegramPublisherUnavailable" => StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = res.Error }),
                _ => BadRequest(new { error = res.Error })
            };
        }

        return NoContent();
    }
}
