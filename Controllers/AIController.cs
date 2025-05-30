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
            var result =
                await aiService.GenerateProblemsAsync(
                    new ChatGptService.StartupInput(dto.Location, dto.ProjectName, dto.Idea), 5);
            return Ok(new { Result = result });
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateSolutions([FromBody] EvaluateDto dto)
        {
            var result = await aiService.EvaluateSolutionsAsync(new ChatGptService.StartupInput(
                dto.StartupInitDto.Location, dto.StartupInitDto.ProjectName,
                dto.StartupInitDto.Idea), dto.ProblemsPayload, dto.UserSolutions);
            return Ok(new { Result = result });
        }
    }
}
