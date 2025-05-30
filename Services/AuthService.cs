using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;

namespace DeathByAIBackend.Services;

public class AuthService(
    IOAuthProvider googleAuthProvider,
    IUserService userService,
    ITokenService tokenService) : IAuthService
{
    public async Task<Uri> GetAuthUrl()
    {
        return await googleAuthProvider.GetAuthUrl();
    }

    public async Task<string> AuthorizeUser(string code)
    {
        var authenticatedUser = await googleAuthProvider.GetAuthenticatedUser(code);

        if (authenticatedUser.Email == null)
            throw new ArgumentException("Email cannot be null", nameof(authenticatedUser.Email));

        var user = await userService.TryGetUserByEmail(authenticatedUser.Email) ?? await userService.CreateUser(new User
        {
            Name = authenticatedUser.Name,
            Email = authenticatedUser.Email,
            AvatarUrl = authenticatedUser.AvatarUrl,
            AuthToken = tokenService.GenerateToken(authenticatedUser.Email)
        });

        return $"{user.AuthToken}";
    }
}