using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Application.Interfaces;

public interface IPasswordResetTokenService
{
    Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(PasswordResetToken token, CancellationToken ct = default);
    Task InvalidateAllForUserAsync(Guid userId, CancellationToken ct = default);
}
