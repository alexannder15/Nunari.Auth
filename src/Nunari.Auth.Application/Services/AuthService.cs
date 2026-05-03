using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nunari.Auth.Application.Exceptions;
using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Dtos.Requests;
using Nunari.Auth.Domain.Dtos.Responses;
using Nunari.Auth.Domain.Enums;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Identity;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Nunari.Auth.Application.Services;

internal class AuthService(
    UserManager<User> userManager,
    IUserService userService,
    IJwtService jwtService,
    IEnumerable<IOAuthService> OAuthServices,
    ILogger<AuthService> logger,
    IRefreshTokenService refreshTokenService,
    IPasswordResetTokenService passwordResetTokenService,
    IValidator<CreateUserRequest> createUserValidator,
    IValidator<LoginUserRequest> loginUserValidator)
    : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(CreateUserRequest request, string deviceInfo)
    {
        var result = await createUserValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var userByEmail = await userManager.FindByEmailAsync(request.Email);

        if (userByEmail != null)
            throw new EmailAlreadyExistException("Email already exist");

        var user = User.CreateWithPassword(request.Email);

        var isCreated = await userManager.CreateAsync(user, request.Password);

        if (!isCreated.Succeeded)
            throw new UnhandledException($"Something was wrong with Register!: {JsonSerializer.Serialize(isCreated.Errors)}");

        string rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, rawRefreshToken, deviceInfo);

        await refreshTokenService.AddAsync(refreshToken);

        var accessToken = jwtService.GenerateAccessToken(user);

        return new AuthResponse(accessToken, rawRefreshToken,
            new UserDto(user.Id, user.Email, user.PictureUrl, user.EmailConfirmed)
            );
    }

    public async Task<AuthResponse> LoginAsync(LoginUserRequest request, string deviceInfo)
    {
        var result = await loginUserValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        User? user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new EmailNotFoundException("User doesn't exist");

        bool isCorrect = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isCorrect)
            throw new InvalidCredentialsException("Invalid credentials");

        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, rawRefreshToken, deviceInfo);
        await refreshTokenService.AddAsync(refreshToken);

        var accessToken = jwtService.GenerateAccessToken(user);

        return new AuthResponse(accessToken, rawRefreshToken,
            new UserDto(user.Id, user.Email, user.PictureUrl, user.EmailConfirmed)
            );
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        User? user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new EmailNotFoundException("User doesn't exist");

        await passwordResetTokenService.InvalidateAllForUserAsync(user.Id);

        // Generate a cryptographically random token
        var rawToken = GenerateSecureToken();
        var tokenHash = HashToken(rawToken);

        var resetToken = PasswordResetToken.Create(user.Id, tokenHash);
        await passwordResetTokenService.AddAsync(resetToken);

        // In production: build this URL from config
        var resetLink = $"https://app.nunari.com/reset-password?token={Uri.EscapeDataString(rawToken)}";
        //_ = email.SendPasswordResetAsync(user.Email, user.DisplayName, resetLink, ct);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var tokenHash = HashToken(request.Token);

        var resetToken = await passwordResetTokenService.FindByTokenHashAsync(tokenHash)
            ?? throw new UnauthorizedException("Invalid or expired reset token.");

        if (!resetToken.IsValid)
            throw new UnauthorizedException("Invalid or expired reset token.");

        var user = await userManager.FindByIdAsync(resetToken.UserId.ToString())
            ?? throw new UnauthorizedException("User not found");

        if (!user.State.Equals(UserState.Active))
            throw new UnauthorizedException("This account has been deactivated.");

        var resetResult = await userManager.ResetPasswordAsync(user, resetToken.TokenHash, request.NewPassword);

        if (!resetResult.Succeeded)
            throw new UnhandledException($"Something was wrong with ResetPassword!: {JsonSerializer.Serialize(resetResult.Errors)}");

        await refreshTokenService.RevokeAllForUserAsync(user.Id);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string deviceInfo)
    {
        var existing = await refreshTokenService.FindByTokenAsync(request.Token)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existing.IsActive)
            throw new UnauthorizedException("Refresh token is expired or revoked.");

        var user = await userManager.FindByIdAsync(existing.UserId.ToString())
            ?? throw new UnauthorizedException("User not found");

        if (!user.State.Equals(UserState.Active))
            throw new UnauthorizedException("This account has been deactivated.");

        // Rotate: revoke old, issue new
        var newRawToken = jwtService.GenerateRefreshToken();
        existing.Revoke(replacedByToken: newRawToken);

        var newRefreshToken = RefreshToken.Create(user.Id, newRawToken, deviceInfo);
        await refreshTokenService.AddAsync(newRefreshToken);

        var accessToken = jwtService.GenerateAccessToken(user);

        return new AuthResponse(accessToken, newRawToken,
            new UserDto(user.Id, user.Email, user.PictureUrl, user.EmailConfirmed)
            );
    }

    public async Task<AuthResponse> OAuthLogin(OAuthRequest request, string deviceInfo)
    {
        var validator = OAuthServices.FirstOrDefault(v => v.GetType().Name
           .StartsWith(request.Provider, StringComparison.OrdinalIgnoreCase))
           ?? throw new NotFoundException($"No validator registered for provider '{request.Provider}'.");

        var payload = await validator.ValidateAsync(request.IdToken)
            ?? throw new UnauthorizedException("OAuth token is invalid or expired.");

        // Try to find existing user by provider ID first, fall back to email
        var user = request.Provider == "google"
            ? await userService.FindByGoogleIdAsync(request.Provider)
              ?? await userManager.FindByEmailAsync(payload.Email)
            : await userService.FindByAppleIdAsync(request.Provider)
              ?? await userManager.FindByEmailAsync(payload.Email);

        var isNewUser = user is null;

        if (user is null)
        {
            // First-time sign-in: create account
            user = User.CreateWithOAuth(
                payload.Email,
                //payload.DisplayName ?? payload.Email.Split('@')[0],
                payload.PictureUrl,
                googleId: request.Provider == "google" ? payload.ProviderId : null,
                appleId: request.Provider == "apple" ? payload.ProviderId : null
            );
            await userManager.CreateAsync(user);
        }
        else
        {
            // Returning user: link provider ID if not already linked
            if (request.Provider == "google" && user.GoogleId is null) user.LinkGoogle(payload.ProviderId);
            if (request.Provider == "apple" && user.AppleId is null) user.LinkApple(payload.ProviderId);
        }

        if (!user.State.Equals(UserState.Active))
            throw new UnauthorizedException("This account has been deactivated.");

        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, rawRefreshToken, deviceInfo);
        await refreshTokenService.AddAsync(refreshToken);

        //if (isNewUser)
        //    _ = events.PublishUserRegisteredAsync(user.Id, user.Email, user.DisplayName);

        var accessToken = jwtService.GenerateAccessToken(user);

        return new AuthResponse(
            accessToken,
            rawRefreshToken,
            new UserDto(user.Id, user.Email, user.PictureUrl, user.EmailConfirmed)
        );
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string HashToken(string raw)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
