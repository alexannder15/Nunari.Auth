using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nunari.Auth.Domain.Dtos;
using Nunari.Auth.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Nunari.Auth.Infrastructure.Services;

public class GoogleOptions
{
    public const string Section = "OAuth:Google";
    public string ClientId { get; init; } = string.Empty;
}

public class GoogleTokenValidator(IOptions<GoogleOptions> opts) : IOAuthService
{
    private readonly string _clientId = opts.Value.ClientId;

    public async Task<OAuthPayload?> ValidateAsync(string idToken, CancellationToken ct = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return new OAuthPayload(payload.Subject, payload.Email, payload.Name, payload.Picture);
        }
        catch
        {
            return null;
        }
    }
}

// ── Apple ─────────────────────────────────────────────────────────────────────

public class AppleOptions
{
    public const string Section = "OAuth:Apple";
    public string TeamId { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty; // Your app's bundle ID
}

public class AppleTokenValidator(IOptions<AppleOptions> opts, IHttpClientFactory httpFactory)
    : IOAuthService
{
    private readonly string _clientId = opts.Value.ClientId;
    private const string AppleKeysUrl = "https://appleid.apple.com/auth/keys";

    public async Task<OAuthPayload?> ValidateAsync(string idToken, CancellationToken ct = default)
    {
        try
        {
            using var http = httpFactory.CreateClient();
            var jwks = await http.GetFromJsonAsync<AppleJwks>(AppleKeysUrl, ct)
                       ?? throw new InvalidOperationException("Failed to fetch Apple public keys.");

            var handler = new JwtSecurityTokenHandler();
            var unvalidated = handler.ReadJwtToken(idToken);
            var kid = unvalidated.Header.Kid;

            var appleKey = jwks.Keys.FirstOrDefault(k => k.Kid == kid)
                           ?? throw new SecurityTokenException("Matching Apple public key not found.");

            var rsaParams = new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(appleKey.N),
                Exponent = Base64UrlEncoder.DecodeBytes(appleKey.E)
            };

            using var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var validationParams = new TokenValidationParameters
            {
                ValidIssuer = "https://appleid.apple.com",
                ValidAudience = _clientId,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

            handler.ValidateToken(idToken, validationParams, out var validToken);
            var jwt = (JwtSecurityToken)validToken;

            var sub = jwt.Claims.First(c => c.Type == "sub").Value;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
            // Apple doesn't provide name in the token after the first sign-in
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            return new OAuthPayload(sub, email, name, PictureUrl: null);
        }
        catch
        {
            return null;
        }
    }

    private record AppleJwks(AppleKey[] Keys);
    private record AppleKey(string Kid, string Kty, string Use, string Alg, string N, string E);
}
