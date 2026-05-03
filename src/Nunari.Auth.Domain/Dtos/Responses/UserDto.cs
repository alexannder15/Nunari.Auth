namespace Nunari.Auth.Domain.Dtos.Responses;

public record UserDto(
    Guid Id,
    string Email,
    string? AvatarUrl,
    bool IsEmailVerified
);
