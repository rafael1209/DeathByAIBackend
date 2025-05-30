using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using DeathByAIBackend.Services;
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
            //var result = await aiService.SendTextQueryAsync("TEST");
            var result = await aiService.GenerateProblemsAsync(new ChatGptService.StartupInput("Russia", "Челябинск", "КриптоСтартап", "Я хочу открыть крипто стартап"), 5);
            return Ok(new { sessionId = Guid.NewGuid() });
        }
    }
}
