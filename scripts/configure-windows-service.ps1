$publish = "..\src\TeamsLiveTranscriptionBot\bin\Release\net6.0\publish"
dotnet publish ..\src\TeamsLiveTranscriptionBot\TeamsLiveTranscriptionBot.csproj -c Release

New-Service -Name "TeamsLiveTranscriptionBot" -BinaryPathName "dotnet `"$publish\TeamsLiveTranscriptionBot.dll`"" -DisplayName "TeamsLiveTranscriptionBot" -StartupType Automatic
Start-Service TeamsLiveTranscriptionBot
