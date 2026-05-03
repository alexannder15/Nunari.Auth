using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task RevokeAllForUserAsync(Guid userId);
}
