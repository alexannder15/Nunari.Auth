using Microsoft.EntityFrameworkCore;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Identity;
using Nunari.Auth.Infrastructure.Context;

namespace Nunari.Auth.Infrastructure.Repositories;

public class PasswordResetTokenRepository(AppDbContext dbContext) : Repository<PasswordResetToken>(dbContext), IPasswordResetTokenRepository
{
    public Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => dbContext.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct = default)
        => await dbContext.PasswordResetTokens.AddAsync(token, ct);

    public async Task InvalidateAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await dbContext.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync(ct);
        foreach (var t in tokens) t.MarkUsed();
    }
}
