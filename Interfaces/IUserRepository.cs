using DeathByAIBackend.Models;
using MongoDB.Bson;

namespace DeathByAIBackend.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserById(ObjectId id);
    Task<User?> TryGetUserByEmail(string email);
    Task<User?> TryGetUserByAuthToken(string authToken);
    Task<User> CreateUser(User user);
}