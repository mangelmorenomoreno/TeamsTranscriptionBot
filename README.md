# 🗣️ TeamsLiveTranscriptionBot

**TeamsLiveTranscriptionBot** es un bot desarrollado en **.NET 8 (C#)** que se conecta a reuniones de **Microsoft Teams**, captura el audio en tiempo real y publica la transcripción automáticamente en el chat de la reunión.

El bot utiliza **Microsoft Graph Communications SDK** para manejar las llamadas y la transmisión de audio, y **Azure Speech to Text** para la conversión de voz a texto.

---

## 🚀 Características principales

- ✅ Conexión directa a reuniones de Microsoft Teams.  
- 🎧 Captura de audio en tiempo real.  
- 🧠 Transcripción continua mediante **Azure Cognitive Services Speech to Text**.  
- 💬 Publicación automática del texto transcrito en el chat de Teams.  
- 🔁 Comando `/transcribe on|off` para activar o detener la transcripción.  
- ⚙️ Configuración flexible mediante `appsettings.json`.

---

## 🧩 Estructura del proyecto

```
TeamsLiveTranscriptionBot/
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
├── Controllers/
│   └── BotController.cs
├── Bots/
│   └── TeamsTranscriptionBot.cs
├── Services/
│   ├── GraphCallingService.cs
│   ├── SpeechToTextService.cs
│   ├── ChatPoster.cs
│   └── AudioSocketHandler.cs
├── Models/
│   ├── Options.cs
│   └── TranscriptionModels.cs
├── Media/
│   └── (archivos auxiliares de configuración y logs de audio)
└── TeamsLiveTranscriptionBot.csproj
```

**Componentes clave:**
- **GraphCallingService:** administra las llamadas entrantes y la conexión con la API de medios de Teams.  
- **SpeechToTextService:** convierte el audio en texto utilizando el servicio de Azure Speech.  
- **ChatPoster:** publica las transcripciones generadas en el chat de Teams.  
- **TeamsTranscriptionBot:** maneja los comandos de activación/desactivación.  
- **BotController:** endpoint REST para recibir eventos desde Microsoft Teams.  

---

## ⚙️ Requisitos previos

1. **.NET 8 SDK o superior**
   ```bash
   dotnet --version
   ```

2. **Ngrok** (para exponer localmente el bot a Internet)
   ```bash
   ngrok http 5124
   ```

3. **Cuenta de Azure** con un recurso activo de **Speech Service**.

4. **Aplicación registrada en Microsoft Entra ID (Azure AD)** con permisos:  
   - `Calls.AccessMedia.All`
   - `Calls.JoinGroupCall.All`
   - `Calls.JoinGroupCallAsGuest.All`
   - `OnlineMeetings.ReadWrite`
   - `Chat.ReadWrite`

5. **Permisos consentidos a nivel de administrador**.

---

## 🔧 Configuración

Archivo `appsettings.json`:

```json
{
  "Bot": {
    "MicrosoftAppId": "<APP_ID_BOT>",
    "MicrosoftAppPassword": "<CLIENT_SECRET_BOT>",
    "TenantId": "<TENANT_ID>",
    "ServiceUrl": "https://smba.trafficmanager.net/emea/"
  },
  "GraphCalling": {
    "AppId": "<APP_ID_BOT>",
    "TenantId": "<TENANT_ID>",
    "ClientSecret": "<CLIENT_SECRET>",
    "PublicBaseUrl": "https://<NGROK_SUBDOMAIN>.ngrok-free.app/",
    "CallbackPath": "/api/calling",
    "MediaStartPort": 50000,
    "MediaEndPort": 51000
  },
  "AzureSpeech": {
    "Region": "eastus",
    "Key": "<AZURE_SPEECH_KEY>",
    "Languages": "es-ES"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

---

## ▶️ Ejecución local

1. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

2. **Compilar**
   ```bash
   dotnet build
   ```

3. **Ejecutar**
   ```bash
   dotnet run
   ```

4. **Exponer con Ngrok**
   ```bash
   ngrok http 5124
   ```

   Actualiza la URL generada en `appsettings.json` → `"PublicBaseUrl"` y en el portal de Azure → *Messaging Endpoint*:  
   ```
   https://<ngrok-subdomain>.ngrok-free.app/api/messages
   ```

---

## 💬 Pruebas en Teams

1. Empaqueta el bot como aplicación de Teams (`manifest.json`).  
2. Instálalo en tu entorno de Teams (modo desarrollador).  
3. En el chat del bot, escribe:
   ```
   /transcribe on
   ```
4. Invita al bot a una reunión.  
   Cuando se una, comenzará a transcribir el audio y publicar el texto en el chat.

---

## 🧪 Estado actual del proyecto

- ✅ Compila correctamente en .NET 8.  
- ✅ Se inicializa la plataforma de medios sin errores.  
- ✅ Preparado para conectarse a Microsoft Teams.  
- ✅ Transcribe el audio en tiempo real usando Azure Speech.  
- ⏳ Pendiente: pruebas finales en entorno real de Teams.

---

## 🧑‍💻 Autor

**Miguel Ángel Moreno Moreno**  
Desarrollador Backend  
📧 [miguel.moreno@perceptio.net](mailto:miguel.moreno@perceptio.net)  
🔗 [GitHub: @mangelmorenomoreno](https://github.com/mangelmorenomoreno)

---

## 🪪 Licencia

Proyecto distribuido bajo licencia **MIT**.
