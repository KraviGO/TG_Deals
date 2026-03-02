using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Publishers.Presentation.Debug;

[ApiController]
[Route("api/v1/debug")]
public sealed class DebugController : ControllerBase
{
    [Authorize(Policy = "Publisher")]
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        var sub = User.FindFirst("sub")?.Value;
        var role = User.FindFirst("role")?.Value;
        var email = User.FindFirst("email")?.Value;

        return Ok(new { sub, email, role, service = "Publishers" });
    }
}
