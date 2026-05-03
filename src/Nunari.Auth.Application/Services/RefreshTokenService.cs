using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Application.Services;

public class RefreshTokenService(IRefreshTokenRepository repository) : IRefreshTokenService
{
    public Task<RefreshToken?> FindByTokenAsync(string token)
        => repository.FindByTokenAsync(token);

    public async Task AddAsync(RefreshToken token)
        => await repository.AddAsync(token);

    public async Task RevokeAllForUserAsync(Guid userId) =>
        await repository.RevokeAllForUserAsync(userId);
}
