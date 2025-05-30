namespace DeathByAIBackend.Models;

public class AuthenticatedUser(string name, string? email, string avatarUrl)
{
    public string Name { get; set; } = name;
    public string? Email { get; set; } = email;
    public string AvatarUrl { get; set; } = avatarUrl;
}