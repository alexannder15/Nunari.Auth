namespace Nunari.Auth.Domain.Dtos;

public record OAuthPayload(string ProviderId, string Email, string? DisplayName, string? PictureUrl);
