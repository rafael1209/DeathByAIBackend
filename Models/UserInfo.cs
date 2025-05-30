namespace DeathByAIBackend.Models;

public class UserInfo
{
    public string Id { get; set; }
    public string Nickname { get; set; }
    public string AvatarUrl { get; set; }
    public int Points { get; set; }
    public int GreenPoints { get; set; }
}