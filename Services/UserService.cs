using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using MongoDB.Bson;

namespace DeathByAIBackend.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> GetUserById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid ObjectId format", nameof(id));

        var user = await userRepository.GetUserById(objectId);

        return user;
    }

    public async Task<UserInfo> GetUserInfo(User user)
    {
        var userInfo = new UserInfo
        {
            Id = user.Id.ToString(),
            Nickname = user.Name,
            AvatarUrl = user.AvatarUrl,
            Points = user.Points,
            GreenPoints = user.GreenPoints
        };

        return userInfo;
    }

    public async Task<User?> TryGetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        var user = await userRepository.TryGetUserByEmail(email);

        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        var createdUser = await userRepository.CreateUser(user);

        return createdUser;
    }

    public async Task<Leaderboard> GetLeaderboard()
    {
        var leaders = await userRepository.GetLeaders();

        var entries = leaders.Select(u => new LeaderboardEntry
        {
            User = new UserDto
            {
                Id = u.Id.ToString(),
                Username = u.Name,
                AvatarUrl = u.AvatarUrl
            },
            Score = u.Points,
            GreenPoints = u.GreenPoints
        }).ToList();

        var leaderboard = new Leaderboard
        {
            LastUpdated = DateTime.UtcNow,
            Entries = entries
        };

        return leaderboard;
    }
}