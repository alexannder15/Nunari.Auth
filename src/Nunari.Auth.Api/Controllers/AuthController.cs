using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nunari.Auth.Api.Controllers.Common;
using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.Dtos.Requests;
using Nunari.Auth.Domain.Dtos.Responses;

namespace Nunari.Auth.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : SecurityJwtController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var deviceInfo = Request.Headers.UserAgent.ToString();

        var result = await authService.RegisterAsync(request, deviceInfo);

        var response = new ResponseDto<AuthResponse>
        {
            Code = "200.02.001",
            Message = "User created",
            Data = result,
        };

        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var deviceInfo = Request.Headers.UserAgent.ToString();

        var result = await authService.LoginAsync(request, deviceInfo);

        var response = new ResponseDto<AuthResponse>
        {
            Code = "200.02.002",
            Message = "User logged in successfully",
            Data = result,
        };

        return Ok(response);
    }

    [HttpPost("oauth")]
    public async Task<IActionResult> OAuth(OAuthRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var deviceInfo = Request.Headers.UserAgent.ToString();

        var result = await authService.OAuthLogin(request, deviceInfo);

        var response = new ResponseDto<AuthResponse>
        {
            Code = "200.02.002",
            Message = "User logged in successfully",
            Data = result,
        };
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var deviceInfo = Request.Headers.UserAgent.ToString();

        var result = await authService.RefreshTokenAsync(request, deviceInfo);

        var response = new ResponseDto<AuthResponse>
        {
            Code = "200.02.002",
            Message = "User refresh token successfully",
            Data = result,
        };
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await authService.ForgotPasswordAsync(request);

        var response = new ResponseDto<string>
        {
            Code = "200.02.002",
            Message = "User forgot password successfully"
        };
        return Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await authService.ResetPasswordAsync(request);

        var response = new ResponseDto<string>
        {
            Code = "200.02.002",
            Message = "User reset pasword successfully"
        };
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await authService.RefreshTokenAsync(new RefreshTokenRequest(request.RefreshToken), string.Empty);

        var response = new ResponseDto<string>
        {
            Code = "200.02.002",
            Message = "User logged out successfully"
        };
        return Ok(response);
    }
}
