using DeathByAIBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeathByAIBackend.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserById(ObjectId id);
    Task<User?> TryGetUserByEmail(string email);
    Task<User?> TryGetUserByAuthToken(string authToken);
    Task<User> CreateUser(User user);
    Task<List<User>> GetLeaders();

    Task<UpdateResult> AddPointsAsync(string authToken, int addPoints, int addGreen);
}