using Microsoft.AspNetCore.Mvc;

namespace ChannelCatalog.Presentation.Internal;

[ApiController]
[Route("api/v1/internal")]
public sealed class InternalDebugController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { ok = true, service = "ChannelCatalog", internalAuth = true });
}
