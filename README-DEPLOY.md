# TeamsLiveTranscriptionBot — Guía de despliegue (Windows Server)

> Requisitos clave de plataforma: bot de **media en tiempo real** con Microsoft Graph (Cloud Communications). Debe ejecutarse en **Windows Server** (VM/VMSS/AKS Windows). Exponer **443**, **8445** y rango **UDP** para audio (por ejemplo 50000–50019). Azure Speech para STT.

## 1) Recursos que necesitas

- **App Registration (Entra ID)**: 1 aplicación (App ID = MicrosoftAppId).
- **Azure Bot (Bot Channels Registration)** vinculado a esa App.
- **Azure Speech** (Key + Region).
- **VM Windows Server con IP pública** y dominio apuntando a su IP.
- Certificado **TLS** para el dominio (IIS/Kestrel) y **certificado de instancia** para la **Media Platform** (mTLS, thumbprint en `appsettings.json`).

## 2) Permisos (Entra ID → API Permissions → Microsoft Graph → Application)

Agrega y concede admin consent a:
- `Calls.JoinGroupCall.All`
- `Calls.JoinGroupCallAsGuest.All`
- `Calls.AccessMedia.All`
- `OnlineMeetings.Read.All` (o `ReadWrite` si crearas/actualizaras reuniones)

> **Mensajes de chat**: no se envían por Graph con permisos de aplicación (no soportado para uso normal). El bot publica en el chat por **Bot Framework** (vía `ChatPoster`).

## 3) Azure Bot (Channels)

- **Messaging endpoint**: `https://<tu-dominio>/api/messages`
- **Microsoft Teams (Calling)**: habilitar **Enable calling** y configurar **Webhook**: `https://<tu-dominio>/api/calling`

## 4) Certificados

### 4.1 Certificado TLS (sitio web)
- Instálalo en la VM (IIS o usa Kestrel con `.pfx`).
- Asegúrate de servir `https://<tu-dominio>` en 443.

### 4.2 Certificado de instancia (Media Platform)
- Instálalo en **Local Machine → Personal**.
- Copia el **Thumbprint** y pégalo en `GraphCalling.InstanceCertThumbprint`.
- Concede permisos de lectura de la clave privada al usuario que ejecute el servicio (por ejemplo `NT SERVICE\TeamsLiveTranscriptionBot` o la cuenta de servicio que uses).

## 5) Puertos / Firewall / NSG

- TCP **443** (HTTPS) y **8445** (control media).
- **UDP** rango 50000–50019 (o el que definas en `appsettings.json`).
- Asegúralo en el firewall de Windows y en la NSG de Azure.

## 6) appsettings

Copia `appsettings.example.json` a `appsettings.json` y rellena:
- `Bot.MicrosoftAppId`, `Bot.MicrosoftAppPassword`, `Bot.TenantId`
- `GraphCalling.AppId` (igual a MicrosoftAppId), `TenantId`, `PublicBaseUrl`, `CallbackPath`, `InstanceCertThumbprint`, `MediaStartPort`, `MediaEndPort`
- `AzureSpeech.Region`, `AzureSpeech.Key`, `AzureSpeech.Languages`

> Recomendado: guarda secretos críticos como **variables de entorno** y deja placeholders en `appsettings.json`.

## 7) Variables de entorno

Ejecuta:
```powershell
.\scripts\set-env.ps1 `
  -AppId "<GUID_APP_ID_BOT>" `
  -ClientSecret "<CLIENT_SECRET_BOT>" `
  -SpeechKey "<SPEECH_KEY>" `
  -SpeechRegion "<SPEECH_REGION>"
