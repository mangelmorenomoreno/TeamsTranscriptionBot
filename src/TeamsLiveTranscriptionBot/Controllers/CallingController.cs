using Microsoft.AspNetCore.Mvc;
using TeamsLiveTranscriptionBot.Services;
using System.Threading.Tasks;

namespace TeamsLiveTranscriptionBot.Controllers
{
    [ApiController]
    [Route("api/calling")]
    public class CallingController : ControllerBase
    {
        private readonly GraphCallingService _service;
        public CallingController(GraphCallingService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> OnNotification()
        {
            try
            {
            // El SDK valida la solicitud y enruta internamente a los handlers
            await _service.HandleCallbackAsync(Request);
            return Ok();
         }
            catch (Exception ex)
        {
            // loggea según tu logger
            return StatusCode(500, new { error = "calling_callback_failed", detail = ex.Message });
        }
        
        }
    }
}
