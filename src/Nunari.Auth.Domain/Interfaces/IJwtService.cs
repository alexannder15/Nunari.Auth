using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();
}