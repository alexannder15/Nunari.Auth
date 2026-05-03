using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Interfaces;

namespace Nunari.Auth.Application.Services;

internal class UserService(IUserRepository repository) : IUserService
{
    public async Task<User?> FindByGoogleIdAsync(string googleId) =>
        await repository.FindByGoogleIdAsync(googleId);
    public async Task<User?> FindByAppleIdAsync(string appleId) =>
        await repository.FindByAppleIdAsync(appleId);
}
