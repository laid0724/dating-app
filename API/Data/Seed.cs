using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json"); // read seed data
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData); // converts the json to c# objects
            
            // these users still need passwords, so we generate them all ass 'devonlypassword' here:

            foreach (var user in users)
            {
                // using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("devonlypassword"));
                // user.PasswordSalt = hmac.Key;

                await context.Users.AddAsync(user);
            }

            await context.SaveChangesAsync();
        }
    }
}