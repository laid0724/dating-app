using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;

        public AuthRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash; // get this from 'out'
            user.PasswordSalt = passwordSalt; // get this from 'out'

            await context.Users.AddAsync(user); // ? add user entity to db
            await context.SaveChangesAsync(); // ? save changes to db

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
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

        public async Task<User> Login(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.Username == username);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (
              // ? pass passwordSalt (the key of the hmac) back into HMACSHA512 object so it can compute the hash with the salt (key)
              var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)
            )
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // ? check if computedHash matches with user's passwordHash stored in the db
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await context.Users.AnyAsync(user => user.Username == username))
            {
                return true;
            }
            return false;
        }
    }
}