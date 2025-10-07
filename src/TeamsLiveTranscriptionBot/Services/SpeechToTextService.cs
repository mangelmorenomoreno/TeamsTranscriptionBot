using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using System.Text;
using TeamsLiveTranscriptionBot.Media;
using TeamsLiveTranscriptionBot.Models;

namespace TeamsLiveTranscriptionBot.Services
{
    public class SpeechToTextService
    {
        private readonly AzureSpeechOptions _options;

        public SpeechToTextService(IOptions<AzureSpeechOptions> options) => _options = options.Value;

        public (AudioSocketToSpeechStream adapter, Task loop) StartRecognizing(
            string threadId,
            Func<string, Task> onText)
        {
            var adapter = new AudioSocketToSpeechStream();
            var t = Task.Run(async () =>
            {
                var stopCts = new CancellationTokenSource();

                var speechConfig = SpeechConfig.FromSubscription(_options.Key, _options.Region);
                // Auto-detección de idioma
                var langs = _options.Languages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var langConfig = AutoDetectSourceLanguageConfig.FromLanguages(langs);

                using var pushStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1));
                using var audioConfig = AudioConfig.FromStreamInput(pushStream);
                using var recognizer = new SpeechRecognizer(speechConfig, langConfig, audioConfig);

                recognizer.Recognized += async (_, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(e.Result.Text))
                        await onText(e.Result.Text);
                };

                await recognizer.StartContinuousRecognitionAsync();

                var readBuffer = new byte[640 * 10]; // lee en bloques
                while (!stopCts.IsCancellationRequested)
                {
                    var read = adapter.Read(readBuffer, 0, readBuffer.Length, stopCts.Token);
                    if (read > 0)
                        pushStream.Write(readBuffer, read);
                    else
                        await Task.Delay(10);
                }
            });

            return (adapter, t);
        }
    }
}
