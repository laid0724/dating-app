// ? this is for setting up DB connections, forming db structure for migrations, etc. 
// ? You need to install and import entity framework core package for this to work. 

using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    /* 
      this is the required syntax for adding a DbContext class.
      then, you need to inject this into ConfigureServices in Startup.cs
        services.AddDbContext<DataContext>();
    */
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Value> Values { get; set; }
    }
}