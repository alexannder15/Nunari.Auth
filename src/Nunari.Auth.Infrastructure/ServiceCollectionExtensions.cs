using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nunari.Auth.Application.Exceptions;
using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Interfaces;
using Nunari.Auth.Domain.Models.Identity;
using Nunari.Auth.Infrastructure.Context;
using Nunari.Auth.Infrastructure.Interfaces;
using Nunari.Auth.Infrastructure.Messaging;
using Nunari.Auth.Infrastructure.Repositories;
using Nunari.Auth.Infrastructure.Services;
using System.Text;

namespace Nunari.Auth.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureCustomServices(
        this IServiceCollection services
    )
    {
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddRoles<Role>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddCustomAuthenticationJwt(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var secret = configuration.GetSection("JwtConfig:Secret").Value;
                var issuer = configuration.GetSection("JwtConfig:Issuer").Value;
                var audience = configuration.GetSection("JwtConfig:Audience").Value;

                if (secret == null || issuer == null || audience == null)
                    throw new UnhandledException("Something was wrong with AddJwtBearer JwtConfig");

                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true, // for dev
                    ValidateAudience = true, // for dev
                    RequireExpirationTime = true, // for dev
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                };
            });

        return services;
    }

    //ToDo: Add SendGrid service for email sending
    //public static IServiceCollection AddCustomSendGrid(
    //    this IServiceCollection services,
    //    IConfiguration configuration
    //)
    //{
    //    services.Configure<SendGridSettings>(configuration.GetSection("SendGrid"));
    //    services.AddScoped<IEmailService, SendGridEmailService>();

    //    return services;
    //}

    public static IServiceCollection AddCustomRabbitMQServices(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

        return services;
    }
}
public static class UserManagerExtensions
{
    // The 'this' keyword tells C# to attach this method to UserManager<TUser>
    public static async Task<TUser?> FindByGoogleIdAsync<TUser>(this UserManager<TUser> userManager, string googleId)
        where TUser : User
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
    }
}