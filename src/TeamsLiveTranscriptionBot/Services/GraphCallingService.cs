using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Communications.Client;
using Microsoft.Graph.Communications.Client.Authentication;
using Microsoft.Graph.Communications.Common.Telemetry;
using Microsoft.Graph.Communications.Calls;
using Microsoft.Graph.Communications.Calls.Media;
using Microsoft.Skype.Bots.Media;
using System.Net.Http.Headers;
using TeamsLiveTranscriptionBot.Media;
using TeamsLiveTranscriptionBot.Models;
using System.Net;
using System.Linq;
using System.Net.Sockets; 

namespace TeamsLiveTranscriptionBot.Services
{
    public class GraphCallingService
    {
        private readonly GraphCallingOptions _opts;
        private readonly AzureSpeechOptions _speech;
        private readonly TranscriptionSessionManager _sessions;
        private readonly ChatPoster _poster;
        private readonly ConversationMappingStore _store;
        private readonly ILogger<GraphCallingService> _log;

        private ICommunicationsClient? _client;

        public GraphCallingService(
            IOptions<GraphCallingOptions> opts,
            IOptions<AzureSpeechOptions> speech,
            TranscriptionSessionManager sessions,
            ChatPoster poster,
            ConversationMappingStore store,
            ILogger<GraphCallingService> log)
        {
            _opts = opts.Value; _speech = speech.Value; _sessions = sessions;
            _poster = poster; _store = store; _log = log;
        }

        public async Task StartAsync()
{
    var graphLogger = new GraphLogger(nameof(GraphCallingService), redirectToTrace: false);

    // Logger de media (vacío)
    IMediaPlatformLogger? mediaLogger = null;


    // Autenticación (app-only)
    if (string.IsNullOrEmpty(_opts.ClientSecret))
    throw new InvalidOperationException("No se configuró ClientSecret en appsettings.json");

var authProvider = new ClientCredentialAuthProvider(_opts.TenantId, _opts.AppId, _opts.ClientSecret);

var fqdn = new Uri(_opts.PublicBaseUrl).Host;

var mediaPlatform = new MediaPlatformSettings
{
    ApplicationId = _opts.AppId,
    MediaPlatformLogger = mediaLogger,
    MediaPlatformInstanceSettings = new MediaPlatformInstanceSettings
    {
        CertificateThumbprint = _opts.InstanceCertThumbprint,
        InstanceInternalPort = 8445,
        MediaPortRange = new PortRange(
            (uint)_opts.MediaStartPort,
            (uint)_opts.MediaEndPort),
        ServiceFqdn = fqdn,
        InstancePublicIPAddress = IPAddress.Parse(_opts.InstancePublicIPAddress!),
        InstancePublicPort = _opts.MediaStartPort
    }
};




    var builder = new CommunicationsClientBuilder("TeamsLiveTranscriptionBot", _opts.AppId, graphLogger)
        .SetAuthenticationProvider(authProvider)
        .SetNotificationUrl(new Uri($"{_opts.PublicBaseUrl}{_opts.CallbackPath}"))
        .SetServiceBaseUrl(new Uri(_opts.PublicBaseUrl))
        .SetMediaPlatformSettings(mediaPlatform);

    _client = builder.Build();

    // Llamadas entrantes (reunión invita al bot)
    _client.Calls().OnIncoming += async (sender, e) =>
    {
        foreach (var call in e.AddedResources)
        {
            try
            {
                var threadId = call.Resource?.ChatInfo?.ThreadId ?? string.Empty;
                _log.LogInformation("Incoming call. ThreadId={threadId} CallId={callId}", threadId, call.Id);

                var mediaSession = new MediaSession(
                    graphLogger,
                    Guid.NewGuid(),
                    new AudioSocketSettings
                    {
                        StreamDirections = StreamDirection.Recvonly,
                        SupportedAudioFormat = AudioFormat.Pcm16K
                    }
                );

                await call.AnswerAsync(mediaSession);
                WireUpAudio(call, mediaSession, threadId);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error answering incoming call");
            }
        }
    };

    await Task.CompletedTask;
}


        public async Task HandleCallbackAsync(HttpRequest request)
        {
            // El SDK enruta por headers, pero debemos leer el cuerpo para no dejarlo abierto.
            using var reader = new StreamReader(request.Body);
            _ = await reader.ReadToEndAsync();
        }

        private void WireUpAudio(ICall call, ILocalMediaSession mediaSession, string threadId)
        {
            var audioSocket = mediaSession.AudioSocket;

            // Instancia de Speech dentro de este contexto
            var speech = new SpeechToTextService(Microsoft.Extensions.Options.Options.Create(_speech));
            var (adapter, loop) = speech.StartRecognizing(threadId, async text =>
            {
                if (_sessions.IsEnabled(threadId))
                    await _poster.PostAsync(threadId, $"📝 {text}");
            });

            audioSocket.AudioMediaReceived += adapter.OnAudioFrame;

            // Cuando la llamada termine, liberamos recursos
            call.OnUpdated += (s, e) =>
            {
                try
                {
                    // Si ya no hay recursos de media o el estado cambió, cerramos el adaptador
                    var state = call.Resource?.State?.ToString() ?? string.Empty;
                    if (string.Equals(state, "terminated", StringComparison.OrdinalIgnoreCase))
                    {
                        adapter.Dispose();
                    }
                }
                catch { /* best effort */ }
            };
        }

        // Autenticación del cliente (app-only) para el Calling SDK
        private class ClientCredentialAuthProvider : IRequestAuthenticationProvider
{
    private readonly string _tenant;
    private readonly Microsoft.Identity.Client.IConfidentialClientApplication _msal;

    public ClientCredentialAuthProvider(string tenantId, string clientId, string clientSecret)
    {
        _tenant = tenantId;
        _msal = Microsoft.Identity.Client.ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithTenantId(tenantId)
            .WithClientSecret(clientSecret)
            .Build();
    }

    public async Task AuthenticateOutboundRequestAsync(HttpRequestMessage request, string tenant)
    {
        var scope = "https://graph.microsoft.com/.default";
        var token = await _msal.AcquireTokenForClient(new[] { scope }).ExecuteAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    public Task<RequestValidationResult> ValidateInboundRequestAsync(HttpRequestMessage request)
    {
        return Task.FromResult(new RequestValidationResult { IsValid = true, TenantId = _tenant });
    }
}
    }
}
