using System.Collections.Concurrent;

namespace TeamsLiveTranscriptionBot.Services
{
    public class TranscriptionSessionManager
    {
        private readonly ConcurrentDictionary<string, bool> _enabledByThread = new();

        public bool IsEnabled(string threadId) => _enabledByThread.TryGetValue(threadId, out var on) ? on : true;
        public void EnableForThread(string threadId, bool enabled) => _enabledByThread[threadId] = enabled;
    }
}
