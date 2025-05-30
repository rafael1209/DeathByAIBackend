namespace DeathByAIBackend.Models;

public class Leaderboard
{
    public List<LeaderboardEntry> Entries { get; set; } = [];
    public DateTime LastUpdated { get; set; }
}