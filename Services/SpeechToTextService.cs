using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace TeamsTranscriptionBot.Services
{
    public class SpeechToTextService
    {
        private readonly SpeechConfig _config;

        public SpeechToTextService(string apiKey, string region)
        {
            _config = SpeechConfig.FromSubscription(apiKey, region);
            _config.SpeechRecognitionLanguage = "es-CO"; // Cambia si necesitas otro idioma
        }

        public async Task<string> TranscribirAudioAsync(Stream audioStream)
        {
            var streamFormat = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);
            var audioInput = AudioConfig.FromStreamInput(new BinaryAudioStreamReader(audioStream), streamFormat);

            using var recognizer = new SpeechRecognizer(_config, audioInput);
            var result = await recognizer.RecognizeOnceAsync();
            return result.Text;
        }
    }
}
