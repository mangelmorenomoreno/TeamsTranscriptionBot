using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using TeamsLiveTranscriptionBot.Models;

namespace TeamsLiveTranscriptionBot.Services
{
    public class ChatPoster
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IOptions<BotOptions> _options;
        private readonly ConversationMappingStore _store;

        public ChatPoster(IBotFrameworkHttpAdapter adapter, IOptions<BotOptions> options, ConversationMappingStore store)
        {
            _adapter = adapter; _options = options; _store = store;
        }

        public async Task PostAsync(string threadId, string text)
        {
            if (!_store.TryGet(threadId, out var reference)) return;
            await ((CloudAdapter)_adapter).ContinueConversationAsync(
                _options.Value.MicrosoftAppId,
                reference,
                async (turn, ct) => await turn.SendActivityAsync(MessageFactory.Text(text), ct),
                default);
        }
    }
}
