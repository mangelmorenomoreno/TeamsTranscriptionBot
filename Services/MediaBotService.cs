using Microsoft.Graph.Communications.Calls;
using Microsoft.Graph.Communications.Calls.Media;
using Microsoft.Graph.Communications.Client;
using Microsoft.Skype.Bots.Media;
using System;
using System.Threading.Tasks;

namespace TeamsTranscriptionBot.Services
{
    public class MediaBotService
    {
        private readonly ICall _call;

        public MediaBotService(ICall call)
        {
            _call = call;
        }

        public Task OnAudioMediaReceived(AudioMediaBuffer buffer)
        {
            // Aquí puedes procesar los datos de audio en tiempo real
            // Por ejemplo, convertir a texto usando Azure Speech
            Console.WriteLine($"Audio recibido: duración {buffer.Length} bytes");
            return Task.CompletedTask;
        }

        public void StartAudioStream()
        {
            Console.WriteLine("Iniciando flujo de audio...");
            // Aquí es donde integrarías el flujo hacia Azure Speech
        }

        public void StopAudioStream()
        {
            Console.WriteLine("Deteniendo flujo de audio...");
        }
    }
}
