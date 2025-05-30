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
            var result = await aiService.GenerateProblemsAsync(new ChatGptService.StartupInput(dto.Location, dto.ProjectName, dto.Idea), 5);
            return Ok(new { Result = result });
        }
    }
}
