namespace DeathByAIBackend.Interfaces
{
    public interface IAIService
    {
        Task<string> SendTextQueryAsync(string query);
    }
}
