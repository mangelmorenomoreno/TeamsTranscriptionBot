using Microsoft.AspNetCore.Mvc;

namespace TeamsLiveTranscriptionBot.Controllers
{
    [ApiController]
    [Route("")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { ok = true, service = "TeamsLiveTranscriptionBot" });

        [HttpGet("api/health")]
        public IActionResult Health() => Ok("OK");
    }
}
