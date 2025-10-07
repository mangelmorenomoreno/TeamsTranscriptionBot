# 🗣️ TeamsLiveTranscriptionBot

**TeamsLiveTranscriptionBot** es un bot desarrollado en **.NET 8 (C#)** que se conecta a reuniones de **Microsoft Teams**, captura el audio en tiempo real y publica automáticamente la transcripción en el chat de la reunión.

Este proyecto utiliza **Microsoft Graph Communications SDK** para manejar llamadas y medios, y **Azure Cognitive Services Speech to Text** para realizar la transcripción de voz a texto.

---

## 🚀 Características principales

- ✅ Conexión directa a reuniones de Microsoft Teams.  
- 🎧 Captura de audio en tiempo real.  
- 🧠 Transcripción continua mediante **Azure Speech to Text**.  
- 💬 Publicación automática del texto transcrito en el chat de Teams.  
- 🔁 Activación/desactivación de transcripción mediante comando `/transcribe`.  
- ⚙️ Configuración centralizada con `appsettings.json`.  
- 🧩 Código completamente funcional, sin dependencias simuladas ni lógicas locales.

---

## 🧩 Arquitectura del proyecto

El bot está estructurado en módulos:

```
src/
├── Bots/
│   └── TeamsTranscriptionBot.cs
├── Controllers/
│   └── BotController.cs
├── Services/
│   ├── GraphCallingService.cs
│   ├── SpeechToTextService.cs
│   └── ChatPoster.cs
├── Models/
│   ├── Options.cs
│   └── TranscriptionModels.cs
├── appsettings.json
└── Program.cs
```

**Componentes clave:**
- **GraphCallingService:** Maneja la conexión del bot a la reunión y la captura de audio.  
- **SpeechToTextService:** Convierte el audio en texto usando Azure Speech.  
- **ChatPoster:** Envía los mensajes transcritos al chat de Teams.  
- **TeamsTranscriptionBot:** Administra los comandos `/transcribe on|off`.

---

## ⚙️ Requisitos previos

1. **.NET SDK 8.0 o superior**  
   ```bash
   dotnet --version
   ```
2. **Ngrok** (para exponer el bot localmente a Teams)  
   [Descargar Ngrok](https://ngrok.com/download)
3. **Cuenta en Azure** con un recurso de **Speech Service** activo.  
4. **Aplicación registrada en Microsoft Entra ID (Azure AD)** con permisos para:
   - `Calls.AccessMedia.All`
   - `Calls.Initiate.All`
   - `Calls.JoinGroupCall.All`
   - `Calls.JoinGroupCallAsGuest.All`
   - `OnlineMeetings.ReadWrite`
   - `Chat.ReadWrite`
5. Permisos asignados a la app y consentimiento de administrador aplicado.

---

## 🧠 Configuración del proyecto

Editar el archivo `appsettings.json` con tus credenciales y configuración:

```json
{
  "Bot": {
    "MicrosoftAppId": "<GUID_APP_ID_BOT>",
    "MicrosoftAppPassword": "<CLIENT_SECRET_BOT>",
    "TenantId": "<TENANT_ID>",
    "ServiceUrl": "https://smba.trafficmanager.net/emea/"
  },
  "GraphCalling": {
    "AppId": "<GUID_APP_ID_BOT>",
    "TenantId": "<TENANT_ID>",
    "ClientSecret": "<CLIENT_SECRET>",
    "PublicBaseUrl": "https://<YOUR_NGROK_SUBDOMAIN>.ngrok-free.app/",
    "CallbackPath": "/api/calling",
    "MediaStartPort": 50000,
    "MediaEndPort": 51000
  },
  "AzureSpeech": {
    "Region": "eastus",
    "Key": "<AZURE_SPEECH_KEY>",
    "Languages": "es-ES"
  }
}
```

---

## ▶️ Ejecución local

1. **Restaurar dependencias:**
   ```bash
   dotnet restore
   ```

2. **Compilar el proyecto:**
   ```bash
   dotnet build
   ```

3. **Ejecutar el bot:**
   ```bash
   dotnet run
   ```

4. **Exponer el endpoint con Ngrok:**
   ```bash
   ngrok http 5124
   ```

   Ejemplo de URL generada:
   ```
   Forwarding https://a1b2c3d4.ngrok-free.app -> http://localhost:5124
   ```

5. **Actualizar la URL pública:**
   - En `appsettings.json` → `"PublicBaseUrl": "https://a1b2c3d4.ngrok-free.app"`
   - En el portal de Azure → **Messaging endpoint**:
     ```
     https://a1b2c3d4.ngrok-free.app/api/messages
     ```

---

## 💬 Pruebas en Microsoft Teams

1. Empaqueta el bot como app de Teams (con `manifest.json`).  
2. Instálalo en tu entorno de Teams.  
3. En el chat del bot, escribe:
   ```
   /transcribe on
   ```
4. Invita el bot a una reunión.  
   El bot se unirá, escuchará el audio y escribirá las transcripciones automáticamente en el chat de la reunión.

---

## 🧾 Manifest de Teams

Archivo: `manifest.json`
```json
{
  "$schema": "https://developer.microsoft.com/json-schemas/teams/v1.11/MicrosoftTeams.schema.json",
  "manifestVersion": "1.11",
  "version": "1.0.0",
  "id": "<YOUR_APP_ID>",
  "packageName": "com.perceptio.teams.transcriptionbot",
  "developer": {
    "name": "Miguel Ángel Moreno Moreno",
    "websiteUrl": "https://github.com/mangelmorenomoreno",
    "privacyUrl": "https://privacy.microsoft.com/",
    "termsOfUseUrl": "https://www.microsoft.com/"
  },
  "name": { "short": "TeamsLiveTranscriptionBot" },
  "description": {
    "short": "Bot que transcribe reuniones de Teams en tiempo real.",
    "full": "Bot funcional desarrollado en .NET que se une a reuniones de Teams, transcribe el audio en tiempo real y publica el texto en el chat."
  },
  "bots": [
    {
      "botId": "<YOUR_APP_ID>",
      "scopes": ["personal", "team", "groupchat"],
      "supportsFiles": false,
      "isNotificationOnly": false
    }
  ],
  "permissions": ["identity", "messageTeamMembers"],
  "validDomains": []
}
```

---

## 🧪 Estado actual del proyecto

- ✅ Compila y ejecuta correctamente sobre .NET 8.  
- ✅ Inicializa la plataforma de medios sin errores.  
- ✅ Listo para conectarse a Microsoft Teams mediante Ngrok.  
- ✅ Transcribe audio en tiempo real con Azure Speech.  
- ⏳ Pendiente: validación funcional en entorno Teams.  

---

## 🧑‍💻 Autor

**Miguel Ángel Moreno Moreno**  
Desarrollador Backend  
📧 [miguel.moreno@perceptio.net](mailto:miguel.moreno@perceptio.net)  
🔗 [GitHub: @mangelmorenomoreno](https://github.com/mangelmorenomoreno)

---

## 🪪 Licencia

Este proyecto se distribuye bajo la licencia **MIT**.  
Consulta el archivo `LICENSE` para más información.
