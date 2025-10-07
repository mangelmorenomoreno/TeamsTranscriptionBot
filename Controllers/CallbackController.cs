using Microsoft.AspNetCore.Mvc;
using TeamsTranscriptionBot.Services;


[ApiController]
[Route("api/callback")]
public class CallbackController : ControllerBase
{
    private readonly SpeechToTextService _speechService;

    public CallbackController(IConfiguration config)
    {
        var apiKey = config["SpeechApiKey"];
        var region = config["SpeechRegion"];
Console.WriteLine($"✅ Speech API Key: {apiKey}");
Console.WriteLine($"✅ Speech Region: {region}");

        _speechService = new SpeechToTextService(apiKey, region);
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        // Simulación: carga archivo .wav desde disco para transcribirlo (puede ser reemplazado con stream real)
        var fileStream = System.IO.File.OpenRead("audio_converted.wav");
        var result = await _speechService.TranscribirAudioAsync(fileStream);
        return Ok(new { Transcripcion = result });
    }
}
