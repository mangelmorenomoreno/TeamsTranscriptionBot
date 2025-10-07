
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace TeamsTranscriptionBot.Adapters
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(ILogger<BotFrameworkHttpAdapter> logger)
            : base()
        {
            OnTurnError = async (turnContext, exception) =>
            {
                logger.LogError($"Exception: {exception.Message}");
                await turnContext.SendActivityAsync("Lo siento, ha ocurrido un error.");
            };
        }
    }
}
