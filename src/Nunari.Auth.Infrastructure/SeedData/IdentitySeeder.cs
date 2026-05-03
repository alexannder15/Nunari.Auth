using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Infrastructure.SeedData;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        string adminEmail = "alexannder15@hotmail.com";
        string adminPassword = "Admin123!"; // ToDo: Change it for something safer
        string adminRole = "Admin";

        // Create role if it doesn't exist
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new Role(adminRole));

        // Check if the admin user exists
        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser == null)
        {
            var user = User.CreateWithPassword(adminEmail);

            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, adminRole);
            else
            {
                // Optional: handle errors
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error creating admin user: {errors}");
            }
        }
    }
}
