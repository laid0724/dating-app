using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any()) // ? check if there are any users in the DB 
            {
                var userData = File.ReadAllText("Data/UsersSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.Users.Add(user);
                }
                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (
              // ? using() {} will automatically call Dispose() [think unsubscribe in angular] to release this instance of the obj so it doesn't take up resource.
              var hmac = new System.Security.Cryptography.HMACSHA512()
            )
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(
                  // ? ComputeHash() takes in an an array of Bytes, so use system encoding to convert password string to Byte[]:
                  System.Text.Encoding.UTF8.GetBytes(password)
                );
            }
        }
    }
}