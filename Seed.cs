using Microsoft.AspNetCore.Identity;
using Seed.Models;

namespace Seed;

public class Seed
{
    public static async Task InitializeRole(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Seed>();

        var roles = config.GetSection("Seed:Roles").Get<List<string>>();

        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new IdentityRole(role);
                var result = await roleManager.CreateAsync(newRole);
                
                if(result.Succeeded)
                {
                    logger.LogInformation($"New role added: {role}");
                }
                else{
                    logger.LogError($"Role not added: {role}");
                }
            }
            else
            {
                logger.LogWarning($"Role have: {role}");
            }
        }
        
        logger.LogInformation("Role seed ended");
    }
    public static async Task InitializeUsers(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Seed>();

        var users = config.GetSection("Seed:Users").Get<List<UserSeed>>();

        foreach(var user in users)
        {
            var newUser = new IdentityUser(user.Username);

            var result = await userManager.CreateAsync(newUser, user.Password);

            if(result.Succeeded)
            {
                logger.LogInformation($"User {user} created");

                var roleResult = await userManager.AddToRolesAsync(newUser, user.Roles);

                if(roleResult.Succeeded)
                {
                    logger.LogInformation($"For {user.Username} role added ");
                }
                else
                {
                    logger.LogError($"For {user.Username} role not added ");
                }
            } 
            else
            {
               logger.LogError($"For {user.Username} role not added ");
            }
        }
        logger.LogInformation("User seed ended");
    }
}