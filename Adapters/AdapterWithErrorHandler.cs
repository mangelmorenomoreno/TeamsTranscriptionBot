using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
{
    public AdapterWithErrorHandler(ILogger<BotFrameworkHttpAdapter> logger)
        : base()
    {
        OnTurnError = async (turnContext, exception) =>
        {
            // Log del error
            logger.LogError(exception, $"[OnTurnError] Error no controlado: {exception.Message}");

            // Mensaje al usuario
            await turnContext.SendActivityAsync("Lo siento, ocurrió un error y no se pudo completar tu solicitud.");
        };
    }
}
