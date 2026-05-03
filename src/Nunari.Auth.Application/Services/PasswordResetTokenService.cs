using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Application.Services;

internal class PasswordResetTokenService(IPasswordResetTokenRepository repository) : IPasswordResetTokenService
{
    public Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => repository.FindByTokenHashAsync(tokenHash, ct);

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct = default)
        => await repository.AddAsync(token, ct);

    public async Task InvalidateAllForUserAsync(Guid userId, CancellationToken ct = default) =>
        await repository.InvalidateAllForUserAsync(userId, ct);
}
