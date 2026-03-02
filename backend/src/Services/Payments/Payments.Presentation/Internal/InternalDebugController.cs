using Microsoft.AspNetCore.Mvc;

namespace Payments.Presentation.Internal;

[ApiController]
[Route("api/v1/internal")]
public sealed class InternalDebugController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { ok = true, service = "Payments", internalAuth = true });
}
