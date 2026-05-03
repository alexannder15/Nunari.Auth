using Nunari.Auth.Api;
using Nunari.Auth.Application;
using Nunari.Auth.Infrastructure;
using Nunari.Auth.Infrastructure.Context;
using Nunari.Auth.Infrastructure.SeedData;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDbContext<AppDbContext>("auth");
builder.AddRabbitMQClient("rabbitmq");

builder.Services.AddApplicationCustomServices();
builder.Services.AddInfrastructureCustomServices();
builder.Services.AddCustomRabbitMQServices();
builder.Services.AddCustomIdentity();
builder.Services.AddCustomAuthenticationJwt(builder.Configuration);
builder.Services.AddCustomCors();
builder.Services.AddCustomValidators();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // Seed the database with initial data
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
