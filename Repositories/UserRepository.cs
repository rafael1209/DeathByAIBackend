using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeathByAIBackend.Repositories;

public class UserRepository(IMongoDbContext dbContext) : IUserRepository
{
    private readonly IMongoCollection<User> _users = dbContext.UsersCollection;

    public async Task<User> GetUserById(ObjectId id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var user = await _users.Find(filter).FirstOrDefaultAsync();

        if (user == null)
            throw new Exception("User not found");

        return user;
    }

    public async Task<User?> TryGetUserByEmail(string email)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        var user = await _users.Find(filter).FirstOrDefaultAsync();

        return user;
    }

    public async Task<User?> TryGetUserByAuthToken(string authToken)
    {
        var filter = Builders<User>.Filter.Eq(u => u.AuthToken, authToken);
        var user = await _users.Find(filter).FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        await _users.InsertOneAsync(user);

        return user;
    }
}