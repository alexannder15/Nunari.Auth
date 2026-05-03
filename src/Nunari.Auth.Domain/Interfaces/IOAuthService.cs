using Nunari.Auth.Domain.Dtos;

namespace Nunari.Auth.Domain.Interfaces;

public interface IOAuthService
{
    Task<OAuthPayload?> ValidateAsync(string idToken, CancellationToken ct = default);
}
