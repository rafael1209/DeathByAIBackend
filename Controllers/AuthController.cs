using DeathByAIBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeathByAIBackend.Controllers;

[Route("api/v1/auth/google")]
[ApiController]
public class AuthController(IAuthService authService) : Controller
{
    [HttpGet("url")]
    public async Task<IActionResult> GetGoogleAuthUrl()
    {
        try
        {
            var authUrl = await authService.GetAuthUrl();

            return Ok(new { url = authUrl });
        }
        catch (Exception)
        {
            return BadRequest(new
            {
                error = "An error occurred while generating the Google authentication URL. Please try again later."
            });
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        try
        {
            var authToken = await authService.AuthorizeUser(code);

            return Ok(new { authToken });
        }
        catch (Exception)
        {
            return BadRequest(new
            {
                error = "An error occurred while processing the Google authentication callback. Please try again later."
            });
        }
    }
}