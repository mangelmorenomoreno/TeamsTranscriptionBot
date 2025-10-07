using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamsLiveTranscriptionBot.Bots;
using TeamsLiveTranscriptionBot.Services;
using TeamsLiveTranscriptionBot.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("Bot"));
builder.Services.Configure<GraphCallingOptions>(builder.Configuration.GetSection("GraphCalling"));
builder.Services.Configure<AzureSpeechOptions>(builder.Configuration.GetSection("AzureSpeech"));

// Bot Framework

builder.Services.AddTransient<IBot, TeamsTranscriptionBot>();
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp =>
{
    var auth = sp.GetRequiredService<BotFrameworkAuthentication>();
    var logger = sp.GetRequiredService<ILogger<CloudAdapter>>();
    return new CloudAdapter(auth, logger);
});

// Infra de publicación de mensajes proactivos
builder.Services.AddSingleton<ConversationMappingStore>();
builder.Services.AddSingleton<ChatPoster>();

// Speech y sesiones
builder.Services.AddSingleton<SpeechToTextService>();
builder.Services.AddSingleton<TranscriptionSessionManager>();

// Graph Calling (cliente y ciclo de vida de media)
builder.Services.AddSingleton<GraphCallingService>();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Host.UseSerilog();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

// Inicializa el cliente de Graph Calling al levantar
var calling = app.Services.GetRequiredService<GraphCallingService>();
await calling.StartAsync();

app.Run();
