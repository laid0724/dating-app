using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext context;

        public DatingRepository(DataContext context)
        {
            this.context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            context.Add(entity); // ? this will be saved in memory untill SaveAll() is called
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity); // ? this will be removed from memory untill SaveAll() is called
        }

        public async Task<User> GetUser(int id)
        {
            var user = await context.Users.Include(user => user.Photos).FirstOrDefaultAsync(user => user.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await context.Users.Include(user => user.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0; // ? If > 0, there are changes made. if === 0, no changes made.
        }
    }
}