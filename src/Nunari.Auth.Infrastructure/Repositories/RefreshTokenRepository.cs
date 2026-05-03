using Microsoft.EntityFrameworkCore;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Identity;
using Nunari.Auth.Infrastructure.Context;

namespace Nunari.Auth.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext dbContext) : Repository<RefreshToken>(dbContext), IRepository<RefreshToken>, IRefreshTokenRepository
{
    public async Task<RefreshToken?> FindByTokenAsync(string token)
        => await dbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

    public async Task AddAsync(RefreshToken token) =>
        await dbContext.RefreshTokens.AddAsync(token);

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var active = await dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();
        foreach (var t in active) t.Revoke();

        await dbContext.SaveChangesAsync();
    }
}
