using Microsoft.AspNetCore.Mvc;
using TeamsTranscriptionBot.Services;

namespace TeamsTranscriptionBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallbackController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CallbackController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se proporcionó ningún archivo de audio.");
            }

            var speechService = new SpeechToTextService(
                _config["SpeechApiKey"],
                _config["SpeechRegion"]);

            using var stream = file.OpenReadStream();
            var texto = await speechService.TranscribirAudioAsync(stream);

            return Ok(new { transcripcion = texto });
        }
    }
}
