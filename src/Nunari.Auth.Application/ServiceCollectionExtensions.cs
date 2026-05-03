using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Application.Services;

namespace Nunari.Auth.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    public static IServiceCollection AddCustomValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
