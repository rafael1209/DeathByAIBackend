namespace DeathByAIBackend.Models;

public class LeaderboardEntry
{
    public UserDto User { get; set; }
    public int Score { get; set; }
    public int GreenPoints { get; set; }
}