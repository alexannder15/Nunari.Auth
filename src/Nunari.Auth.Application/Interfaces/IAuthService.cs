using Nunari.Auth.Domain.Dtos.Requests;
using Nunari.Auth.Domain.Dtos.Responses;

namespace Nunari.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(CreateUserRequest request, string deviceInfo);
    Task<AuthResponse> LoginAsync(LoginUserRequest request, string deviceInfo);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string deviceInfo);
    Task<AuthResponse> OAuthLogin(OAuthRequest request, string deviceInfo);
}
