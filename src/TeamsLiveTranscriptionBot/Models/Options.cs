namespace TeamsLiveTranscriptionBot.Models
{
    public class BotOptions
    {
        public string MicrosoftAppId { get; set; } = "";
        public string MicrosoftAppPassword { get; set; } = "";
        public string TenantId { get; set; } = "";
        public string ServiceUrl { get; set; } = "";
    }

    public class GraphCallingOptions
    {
        public string AppId { get; set; } = "";
        public string TenantId { get; set; } = "";
        public string? ClientSecret { get; set; } 
        public string PublicBaseUrl { get; set; } = "";
        public string CallbackPath { get; set; } = "/api/calling";
        public string InstanceCertThumbprint { get; set; } = "";
        public int MediaStartPort { get; set; } = 50000;
        public int MediaEndPort { get; set; } = 50019;
        public string? InstancePublicIPAddress { get; set; }
    }

    public class AzureSpeechOptions
    {
        public string Region { get; set; } = "";
        public string Key { get; set; } = "";
        public string Languages { get; set; } = "es-ES,en-US";
    }
}
