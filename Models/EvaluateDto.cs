using static DeathByAIBackend.Services.ChatGptService;

namespace DeathByAIBackend.Models
{
    public class EvaluateDto
    {
        public StartupInitDto StartupInitDto { get; set; } = new StartupInitDto();

        public ProblemsPayload ProblemsPayload { get; set; } = new ProblemsPayload();

        public IEnumerable<UserSolution> UserSolutions { get; set; } = new List<UserSolution>();
    }
}
