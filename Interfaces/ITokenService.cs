namespace DeathByAIBackend.Interfaces;

public interface ITokenService
{
    string GenerateToken(string value);
}