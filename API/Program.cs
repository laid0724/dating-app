using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            /*
                here, we are reformating this so that we can get access to the database
                and inject it into seeders before the rest of the program is spun up.

                also set the program to automatically create db / apply migrations
                
            */
            var host = CreateHostBuilder(args).Build();
            
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DataContext>();

                // this will automatically run dotnet ef database update
                await context.Database.MigrateAsync();

                // run all seeders here:
                await Seed.SeedUsers(context);
            }
            catch(Exception ex)
            {
                // this is to catch all error related to migrations/seeding
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
