using CineVerse.Application.Common;
using CineVerse.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.MVC;

public class DbSeeder
{
    public static async Task SeedData(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        try
        {
            var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<AppRole>>();

            if (await roleManager!.RoleExistsAsync(Roles.Admin) is false)
            {
                var roleResult = await roleManager.CreateAsync(new AppRole() { Name = Roles.Admin });
                if (!roleResult.Succeeded)
                {
                    Console.WriteLine(string.Join(",", roleResult.Errors.Select(e => e.Description)));
                    return;
                }
            }

            if (await roleManager.RoleExistsAsync(Roles.Editor) is false)
            {
                var roleResult = await roleManager.CreateAsync(new AppRole() { Name = Roles.Editor });

                if (!roleResult.Succeeded)
                {
                    Console.WriteLine(string.Join(",", roleResult.Errors.Select(e => e.Description)));
                    return;
                }
            }


            if (await roleManager.RoleExistsAsync(Roles.User) is false)
            {
                var roleResult = await roleManager.CreateAsync(new AppRole() { Name = Roles.User });
                if (!roleResult.Succeeded)
                {
                    Console.WriteLine(string.Join(",", roleResult.Errors.Select(e => e.Description)));
                    return;
                }
            }

            if (!await userManager!.Users.AnyAsync())
            {
                var user = new AppUser()
                {
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = "system-admin",
                    DoB = new DateOnly(2000, 1, 1),
                    Email = "system.admin@cineverse.com",
                    EmailConfirmed = true,
                };

                var userResult = await userManager.CreateAsync(user, "Admin@123");
                if (!userResult.Succeeded)
                {
                    Console.WriteLine(string.Join(",", userResult.Errors.Select(e => e.Description)));
                    return;
                }


                var userRoleResult = await userManager.AddToRoleAsync(user, Roles.Admin);
                if (!userRoleResult.Succeeded)
                {
                    Console.WriteLine(string.Join(",", userRoleResult.Errors.Select(e => e.Description)));
                    return;
                }
            }
        }

        catch (Exception ex) 
        {
            Console.WriteLine("Error : " + ex.Message);
        }
    }
}
