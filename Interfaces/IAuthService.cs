namespace DeathByAIBackend.Interfaces;

public interface IAuthService
{
    Task<Uri> GetAuthUrl();
    Task<string> AuthorizeUser(string code);
}