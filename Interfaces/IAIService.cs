using DeathByAIBackend.Services;

namespace DeathByAIBackend.Interfaces
{
    public interface IAIService
    {
        Task<string> SendTextQueryAsync(string query);

        Task<ChatGptService.ProblemsPayload> GenerateProblemsAsync(
            ChatGptService.StartupInput input,
            int count,
            float temperature = 0f);

        Task<ChatGptService.EvaluationPayload> EvaluateSolutionsAsync(
            ChatGptService.StartupInput startup,
            ChatGptService.ProblemsPayload problems,
            IEnumerable<ChatGptService.UserSolution> solutions,
            float temperature = .7f);
    }
}
