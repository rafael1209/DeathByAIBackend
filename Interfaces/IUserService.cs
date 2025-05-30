using DeathByAIBackend.Models;

namespace DeathByAIBackend.Interfaces;

public interface IUserService
{
    Task<User> GetUserById(string id);
    Task<User?> TryGetUserByEmail(string email);
    Task<User> CreateUser(User user);
}