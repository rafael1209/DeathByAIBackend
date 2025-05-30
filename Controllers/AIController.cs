using DeathByAIBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeathByAIBackend.Controllers
{
    [Route("api/v1/ai")]
    [ApiController]
    public class AIController : Controller
    {
        [HttpPost("start")]
        public IActionResult StartSimulation([FromBody] StartupInitDto dto)
        {
            // сохраняем данные, создаём сессию
            return Ok(new { sessionId = Guid.NewGuid() });
        }
    }
}
