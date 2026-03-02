using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Publishers.Entities.Channels;
using Publishers.Presentation.Channels.Dtos;
using Publishers.UseCases.Channels.CreateChannel;
using Publishers.UseCases.Channels.GetMyChannels;
using Publishers.UseCases.Channels.SetIntakeMode;
using Publishers.UseCases.Channels.UpdateChannel;
using Publishers.UseCases.Channels.Verification.ConfirmVerification;
using Publishers.UseCases.Channels.Verification.StartVerification;

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
    private readonly StartVerificationHandler _startVerify;
    private readonly ConfirmVerificationHandler _confirmVerify;

    public ChannelsController(
        CreateChannelHandler create,
        GetMyChannelsHandler getMy,
        UpdateChannelHandler update,
        SetIntakeModeHandler setMode,
        StartVerificationHandler startVerify,
        ConfirmVerificationHandler confirmVerify)
    {
        _create = create;
        _getMy = getMy;
        _update = update;
        _setMode = setMode;
        _startVerify = startVerify;
        _confirmVerify = confirmVerify;
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    [HttpPost]
    public async Task<ActionResult<ChannelResponseDto>> Create([FromBody] CreateChannelRequestDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        var res = await _create.Handle(new CreateChannelCommand(userId, dto.TelegramChannelId, dto.Title), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        var v = res.Value!;
        return Created("", new ChannelResponseDto(v.ChannelId, v.TelegramChannelId, v.Title, v.IntakeMode.ToString(), v.OwnershipStatus.ToString()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ChannelResponseDto>>> GetMy(CancellationToken ct)
    {
        var userId = GetUserId();
        var res = await _getMy.Handle(new GetMyChannelsQuery(userId), ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });

        return Ok(res.Value!.Select(x =>
            new ChannelResponseDto(x.ChannelId, x.TelegramChannelId, x.Title, x.IntakeMode.ToString(), x.OwnershipStatus.ToString())
        ).ToList());
    }

    [HttpPut("{channelId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid channelId, [FromBody] UpdateChannelRequestDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        var res = await _update.Handle(new UpdateChannelCommand(userId, channelId, dto.TelegramChannelId, dto.Title), ct);
        if (!res.IsSuccess) return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });
        return NoContent();
    }

    [HttpPatch("{channelId:guid}/intake-mode")]
    public async Task<IActionResult> SetIntakeMode([FromRoute] Guid channelId, [FromBody] SetIntakeModeRequestDto dto, CancellationToken ct)
    {
        if (!Enum.TryParse<IntakeMode>(dto.Mode, ignoreCase: true, out var mode))
            return BadRequest(new { error = "InvalidMode" });

        var userId = GetUserId();
        var res = await _setMode.Handle(new SetIntakeModeCommand(userId, channelId, mode), ct);
        if (!res.IsSuccess) return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });
        return NoContent();
    }

    [HttpPost("{channelId:guid}/verify/start")]
    public async Task<ActionResult<StartVerificationResponseDto>> StartVerification([FromRoute] Guid channelId, CancellationToken ct)
    {
        var userId = GetUserId();
        var res = await _startVerify.Handle(new StartVerificationCommand(userId, channelId), ct);
        if (!res.IsSuccess) return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });

        return Ok(new StartVerificationResponseDto(res.Value!.Instruction, res.Value.ExpiresAt));
    }

    [HttpPost("{channelId:guid}/verify/confirm")]
    public async Task<IActionResult> ConfirmVerification([FromRoute] Guid channelId, CancellationToken ct)
    {
        var userId = GetUserId();
        var res = await _confirmVerify.Handle(new ConfirmVerificationCommand(userId, channelId), ct);
        if (!res.IsSuccess) return res.Error == "NotFound" ? NotFound() : BadRequest(new { error = res.Error });
        return NoContent();
    }
}
