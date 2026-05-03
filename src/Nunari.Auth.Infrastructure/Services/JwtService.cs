using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nunari.Auth.Application.Exceptions;
using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.AggregateRoot;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nunari.Auth.Infrastructure.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly string _secret = configuration["JwtConfig:Secret"] ?? throw new UnhandledException("JwtConfig:Secret is missing");
    private readonly string _issuer = configuration["JwtConfig:Issuer"] ?? throw new UnhandledException("JwtConfig:Issuer is missing");
    private readonly string _audience = configuration["JwtConfig:Audience"] ?? throw new UnhandledException("JwtConfig:Audience is missing");
    private readonly int _accessTokenExpiresInMinutes = int.TryParse(configuration["JwtConfig:AccessTokenExpiresInMinutes"], out var __expiresTmp)
        ? __expiresTmp
        : throw new UnhandledException("JwtConfig:ExpiresInMinutes is missing or not a valid integer");

    public string GenerateAccessToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim("full_name", user.FullName),
                    new Claim("email_verified", user.EmailConfirmed.ToString().ToLower()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(
                        JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64
                    ),
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiresInMinutes),
            SigningCredentials = creds,
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return jwtToken;
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
