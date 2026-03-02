using System.Security.Claims;
using Identity.Entities.Users;
using Identity.Presentation.Auth.Dtos;
using Identity.UseCases.Auth.Login;
using Identity.UseCases.Auth.Me;
using Identity.UseCases.Auth.Register;
using Identity.UseCases.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterHandler _register;
    private readonly LoginHandler _login;
    private readonly MeHandler _me;

    public AuthController(RegisterHandler register, LoginHandler login, MeHandler me)
    {
        _register = register;
        _login = login;
        _me = me;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
            return BadRequest(new { error = "InvalidRole" });

        var cmd = new RegisterCommand(dto.Email, dto.Password, role);
        var res = await _register.Handle(cmd, ct);

        if (!res.IsSuccess)
            return res.Error switch
            {
                Errors.EmailAlreadyExists => Conflict(new { error = res.Error }),
                _ => BadRequest(new { error = res.Error })
            };

        return Created("", new RegisterResponseDto(res.Value!.UserId, res.Value.Email, res.Value.Role.ToString()));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        var res = await _login.Handle(new LoginCommand(dto.Email, dto.Password), ct);

        if (!res.IsSuccess)
            return Unauthorized(new { error = res.Error });

        return Ok(new LoginResponseDto(res.Value!.AccessToken, res.Value.TokenType, res.Value.ExpiresInSeconds));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<MeResponseDto>> Me(CancellationToken ct)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var userId))
            return Unauthorized(new { error = "InvalidToken" });

        var res = await _me.Handle(new MeQuery(userId), ct);
        if (!res.IsSuccess) return NotFound();

        return Ok(new MeResponseDto(res.Value!.UserId, res.Value.Email, res.Value.Role.ToString()));
    }
}
