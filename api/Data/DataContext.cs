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
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
    /* 
      * Migrations
        ! need to install dotnet-ef tool and package: Microsoft.EntityFrameworkCore.Design
        ::`dotnet ef migrations add InitialCreate`
        :: to apply migrations, run `dotnet ef database update`

        ? When should you run migrations and update db after initial create?
          * whenever you make changes to your models (create a new class / change existing class properties).
    */
}