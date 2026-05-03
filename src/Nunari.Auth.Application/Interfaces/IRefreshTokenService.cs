using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task RevokeAllForUserAsync(Guid userId);
}
