using DeathByAIBackend.Models;
using MongoDB.Driver;

namespace DeathByAIBackend.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<User> UsersCollection { get; }
}