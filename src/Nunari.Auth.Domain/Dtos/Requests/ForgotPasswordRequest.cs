namespace Nunari.Auth.Domain.Dtos.Requests;

public record ResetPasswordRequest(string Token, string NewPassword);
