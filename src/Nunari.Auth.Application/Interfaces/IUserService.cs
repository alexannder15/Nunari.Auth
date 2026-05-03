using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Application.Interfaces;

public interface IUserService
{
    Task<User?> FindByGoogleIdAsync(string googleId);
    Task<User?> FindByAppleIdAsync(string appleId);
}
