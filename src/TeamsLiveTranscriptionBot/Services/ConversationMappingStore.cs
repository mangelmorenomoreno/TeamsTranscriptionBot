using Microsoft.Bot.Schema;
using System.Collections.Concurrent;

namespace TeamsLiveTranscriptionBot.Services
{
    public class ConversationMappingStore
    {
        private readonly ConcurrentDictionary<string, ConversationReference> _byThread = new();

        public void Upsert(IActivity activity)
        {
            var reference = activity.GetConversationReference();
            var threadId = reference.Conversation?.Id;
            if (!string.IsNullOrEmpty(threadId))
            {
                _byThread[threadId] = reference;
            }
        }

        public string GetThreadId(IActivity activity) => activity.Conversation?.Id ?? string.Empty;

        public bool TryGet(string threadId, out ConversationReference reference)
            => _byThread.TryGetValue(threadId, out reference);
    }
}
