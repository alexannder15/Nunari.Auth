using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByGoogleIdAsync(string googleId);
    Task<User?> FindByAppleIdAsync(string appleId);
}
