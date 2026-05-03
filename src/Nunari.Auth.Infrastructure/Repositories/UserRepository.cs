using Microsoft.EntityFrameworkCore;
using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Infrastructure.Context;

namespace Nunari.Auth.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> FindByGoogleIdAsync(string googleId) =>
        await context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
    public async Task<User?> FindByAppleIdAsync(string appleId) =>
        await context.Users.FirstOrDefaultAsync(u => u.AppleId == appleId);
}
