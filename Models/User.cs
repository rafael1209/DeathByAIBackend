using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DeathByAIBackend.Models;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("email")]
    public required string Email { get; set; }

    [BsonElement("avatarUrl")]
    public required string AvatarUrl { get; set; }

    [BsonElement("authToken")]
    public string AuthToken { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}