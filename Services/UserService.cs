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
}