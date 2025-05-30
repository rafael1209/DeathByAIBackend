using DeathByAIBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeathByAIBackend.Controllers;

[Route("api/v1/leaderboard")]
public class LeaderboardController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var leaderboard = new Leaderboard
        {
            LastUpdated = DateTime.UtcNow,
            Entries =
            [
                new LeaderboardEntry
                {
                    User = new UserDto
                    {
                        Id = "1",
                        Username = "PlayerOne",
                        AvatarUrl = "https://example.com/avatar1.png"
                    },
                    Score = 1000,
                    GreenPoints = 50
                },
                new LeaderboardEntry
                {
                    User = new UserDto
                    {
                        Id = "2",
                        Username = "PlayerTwo",
                        AvatarUrl = "https://example.com/avatar2.png"
                    },
                    Score = 900,
                    GreenPoints = 45
                }
            ]
        };

        return Ok(leaderboard);
    }
}