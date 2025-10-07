using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Communications.Calls;
using Microsoft.Graph.Communications.Client;
using TeamsTranscriptionBot.Services;


[ApiController]
[Route("api/calling/callback")]
public class CallingBotController : ControllerBase
{
private readonly MediaBotService _mediaBotService;


public CallingBotController(MediaBotService mediaBotService)
{
_mediaBotService = mediaBotService;
}


[HttpPost]
public IActionResult OnIncomingCall([FromBody] object payload)
{
// TODO: Parse the call info from payload
_mediaBotService.HandleIncomingCall(payload);
return Ok();
}
}