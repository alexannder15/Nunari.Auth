namespace Nunari.Auth.Domain.Dtos.Responses;

public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);
