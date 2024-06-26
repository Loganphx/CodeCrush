﻿using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task ClearConnections(DataContext context)
    {
        context.Connections.RemoveRange(context.Connections);
        await context.SaveChangesAsync();
    }
    public static async Task SeedUsers(ILogger<Seed> logger, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (!await userManager.Users.AnyAsync())
        {
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
                user.UserName = user.UserName.ToLower();
                user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                // user.PasswordSalt = hmac.Key;
                user.Email = user.UserName + "@gmail.com";

                foreach (var photo in user.Photos) photo.IsApproved = true;

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
        }

        var adminRole = await roleManager.Roles.Where(t => t.Name == "Admin").FirstOrDefaultAsync();
        // var adminRole = newRoles.FirstOrDefault(t => t.Name == "Admin");
        // Console.WriteLine($"NEW ROLES({newRoles.Count}): {string.Join(",", newRoles.Select(t => t.Name))}");
        
         if (adminRole.UserRoles == null || adminRole.UserRoles.Any())
         {
             var admin = new AppUser()
             {
                 UserName = "admin",
                 Created = DateTime.UtcNow,
                 LastActive = DateTime.UtcNow,
                 City = "Unknown",
                 Country = "Unknown",
                 KnownAs = "Admin",
                 DateOfBirth = new DateOnly(1996, 12,20),
                 Email = "admin@gmail.com",
                 Gender = "male",
             };
        
             var result = await userManager.CreateAsync(admin, "Password21");
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
}