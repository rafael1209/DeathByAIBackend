using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Middlewares;
using DeathByAIBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DeathByAIBackend.Controllers;

[Route("api/v1/users/me")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetMe()
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        try
        {
            var userInfo = await userService.GetUserInfo(user);

            return Ok(userInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }
}