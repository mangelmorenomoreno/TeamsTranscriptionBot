param(
  [string]$AppId,
  [string]$ClientSecret, # para Graph (se guarda en env var)
  [string]$SpeechKey,
  [string]$SpeechRegion
)
[System.Environment]::SetEnvironmentVariable("GRAPH_CLIENT_SECRET", $ClientSecret, "Machine")
[System.Environment]::SetEnvironmentVariable("SPEECH_KEY", $SpeechKey, "Machine")
[System.Environment]::SetEnvironmentVariable("SPEECH_REGION", $SpeechRegion, "Machine")
Write-Host "Variables registradas en el sistema."
