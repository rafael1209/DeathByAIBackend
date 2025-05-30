using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeathByAIBackend.Controllers;

[Route("api/v1/leaderboard")]
public class LeaderboardController(IUserService userService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var leaderboard = await userService.GetLeaderboard();

        return Ok(leaderboard);
    }
}