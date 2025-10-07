using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using System.IO;
using TeamsTranscriptionBot.Services;


public class TeamsBot : ActivityHandler, IBot
{
    private readonly ILogger<TeamsBot> _logger;
    private readonly SpeechToTextService _speechService;

    public TeamsBot(ILogger<TeamsBot> logger, IConfiguration config)
    {
        _logger = logger;
        _speechService = new SpeechToTextService(
            config["SpeechApiKey"],
            config["SpeechRegion"]);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        // Suponiendo que recibes un audio (en realidad Teams envía el enlace al audio, no el archivo directamente)
        var audioUrl = turnContext.Activity.Attachments?.FirstOrDefault()?.ContentUrl;
        
        if (!string.IsNullOrEmpty(audioUrl))
        {
            using var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(audioUrl);
            var transcription = await _speechService.TranscribirAudioAsync(stream);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Transcripción: {transcription}"), cancellationToken);
        }
        else
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("Envíame un mensaje de voz para transcribir."), cancellationToken);
        }
    }
}
