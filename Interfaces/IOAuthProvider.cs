using DeathByAIBackend.Models;

namespace DeathByAIBackend.Interfaces;

public interface IOAuthProvider
{
    Task<Uri> GetAuthUrl();
    Task<AuthenticatedUser> GetAuthenticatedUser(string code);
}