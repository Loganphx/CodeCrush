using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(ILogger<Seed> logger, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        var roles = new List<AppRole>
        {
            new AppRole { Name = "Member" },
            new AppRole { Name = "Admin" },
            new AppRole { Name = "Moderator" },
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
        
        Console.WriteLine($"Seeding {users.Count} users.");
        IdentityResult result;
        foreach (var user in users)
        {
            user.UserName     = user.UserName.ToLower();
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            // user.PasswordSalt = hmac.Key;
            user.Email = user.UserName + "@gmail.com";
            
            result = await userManager.CreateAsync(user, "Password21");
            if (!result.Succeeded)
            {
                logger.LogError(result.ToString());
            }

            result = await userManager.AddToRoleAsync(user, "Member");
            if (!result.Succeeded)
            {
                logger.LogError(result.ToString());
            }
        }
        
        var admin = new AppUser()
        {
            UserName = "admin"
        };

        result = await userManager.CreateAsync(admin, "Password21");
        if (!result.Succeeded)
        {
            logger.LogError(result.ToString());
        }

        result = await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        if (!result.Succeeded)
        {
            logger.LogError(result.ToString());
        }
    }
}