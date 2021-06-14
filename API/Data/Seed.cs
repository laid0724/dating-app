using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json"); // read seed data
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData); // converts the json to c# objects

            if (users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
                new AppRole{Name = "Member"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // these users still need passwords, so we generate them all as '1234' here (DEV ONLY!):
            foreach (var user in users)
            {
                // using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("1234"));
                // user.PasswordSalt = hmac.Key;
                // await context.Users.AddAsync(user);

                /* 
                    When using identity to manage user creation,
                    it will take care of the following:
                    - hash pw and salt them several times for you
                    - create and save to db automatically
                */
                await userManager.CreateAsync(user, "1234");
                await userManager.AddToRoleAsync(user, "Member");
            }
            // await context.SaveChangesAsync();

            var admin = new AppUser
            {
                UserName = "admin"
            };

            // dev only pw!!
            await userManager.CreateAsync(admin, "1234");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}