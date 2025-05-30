using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using MongoDB.Driver;

namespace DeathByAIBackend.Databases;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    private const string ConstUsersCollection = "users";

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("MongoDb:ConnectionString"));
        _database = client.GetDatabase(configuration.GetValue<string>("MongoDb:DatabaseName"));
    }

    public IMongoCollection<User> UsersCollection => _database.GetCollection<User>(ConstUsersCollection);
}