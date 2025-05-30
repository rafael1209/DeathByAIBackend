using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeathByAIBackend.Controllers
{
    [Route("api/v1/ai")]
    [ApiController]
    public class AIController(IAIService aiService) : Controller
    {
        [HttpPost("start")]
        public async Task<IActionResult> StartSimulation([FromBody] StartupInitDto dto)
        {
            var result = await aiService.SendTextQueryAsync("TEST");
            return Ok(new { sessionId = Guid.NewGuid() });
        }
    }
}
