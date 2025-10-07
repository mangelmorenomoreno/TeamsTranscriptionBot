using Microsoft.Skype.Bots.Media;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace TeamsLiveTranscriptionBot.Media
{
    /// Adapta AudioMediaReceived (20ms, PCM 16kHz 16-bit) a un buffer continuo para Azure Speech
    public class AudioSocketToSpeechStream : IDisposable
    {
        private readonly BlockingCollection<byte[]> _fifo = new(new ConcurrentQueue<byte[]>());
        private volatile bool _disposed;

        public void OnAudioFrame(object? sender, AudioMediaReceivedEventArgs e)
        {
            if (_disposed) { e.Buffer.Dispose(); return; }
            try
            {
                var managed = new byte[e.Buffer.Length];
                // Copia desde memoria no administrada a byte[]
                Marshal.Copy(e.Buffer.Data, managed, 0, (int)e.Buffer.Length);
                _fifo.Add(managed);
            }
            finally { e.Buffer.Dispose(); }
        }

        public int Read(byte[] buffer, int offset, int count, CancellationToken ct)
        {
            var total = 0;
            while (total < count && !_disposed)
            {
                if (!_fifo.TryTake(out var chunk, 50, ct)) break;
                var toCopy = Math.Min(chunk.Length, count - total);
                Buffer.BlockCopy(chunk, 0, buffer, offset + total, toCopy);
                total += toCopy;
            }
            return total;
        }

        public void Dispose()
        {
            _disposed = true;
            _fifo.CompleteAdding();
        }
    }
}
