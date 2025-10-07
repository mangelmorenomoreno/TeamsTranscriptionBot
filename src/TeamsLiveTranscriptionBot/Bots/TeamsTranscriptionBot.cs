using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TeamsLiveTranscriptionBot.Services;

namespace TeamsLiveTranscriptionBot.Bots
{
    public class TeamsTranscriptionBot : TeamsActivityHandler
    {
        private readonly ConversationMappingStore _store;
        private readonly TranscriptionSessionManager _sessions;

        public TeamsTranscriptionBot(ConversationMappingStore store, TranscriptionSessionManager sessions)
        {
            _store = store;
            _sessions = sessions;
        }

        protected override async Task OnMembersAddedAsync(
    IList<ChannelAccount> membersAdded,
    ITurnContext<IConversationUpdateActivity> turnContext,
    CancellationToken cancellationToken)
{
    // Guarda el ConversationReference para publicación proactiva
    _store.Upsert(turnContext.Activity);

    await turnContext.SendActivityAsync(
        "👋 ¡Hola! Me uniré a la reunión y publicaré transcripciones aquí. " +
        "Escribe `/transcribe on` o `/transcribe off` para activar/desactivar.",
        cancellationToken: cancellationToken);
}

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken ct)
        {
            var text = (turnContext.Activity.Text ?? "").Trim().ToLowerInvariant();
            if (text.StartsWith("/transcribe"))
            {
                var on = text.Contains("on") || text.Contains("start") || text.Contains("activar");
                var off = text.Contains("off") || text.Contains("stop") || text.Contains("desactivar");
                if (on) { _sessions.EnableForThread(_store.GetThreadId(turnContext.Activity), true); await turnContext.SendActivityAsync("✅ Transcripción **activada** para esta reunión.", cancellationToken: ct); return; }
                if (off){ _sessions.EnableForThread(_store.GetThreadId(turnContext.Activity), false); await turnContext.SendActivityAsync("⏸️ Transcripción **desactivada** para esta reunión.", cancellationToken: ct); return; }
                await turnContext.SendActivityAsync("Usa: `/transcribe on` o `/transcribe off`.", cancellationToken: ct);
            }
        }
    }
}
