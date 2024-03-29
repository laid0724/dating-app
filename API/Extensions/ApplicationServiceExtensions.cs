using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    // we are aggregating all application services here, so they are more easily managed.
    // extensions must be static, meaning that it doesnt need to be instantiated as a new class to be used.
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<PresenceTracker>();

            /*
                To get access to your cloudinary account, you will need the following settings in your appsettings.json:
                    
                    "CloudinarySettings": {
                        "CloudName": "",
                        "ApiKey": "",
                        "ApiSecret": ""
                    }
            */
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // lifetime: how long should the service be alive for once it is created. 3 options:
            // 1. singleton: created once and lasts through he lifetime of the application, until application is shut down
            // 2. scoped: scoped to the lifetime of the http request - when a requests comes in and we have this service injected into that particular controller, 
            //      then a new instance of this service is created, and when request finishes it is disposed.
            // 3. transient: created and destroyed as soon as the method is finished

            // most appropriate for http requests and for generating token, used most of the time for web apis.
            // provide interface and implementation:
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>(); // this will inject all repositories now

            services.AddScoped<IPhotoService, PhotoService>();

            services.AddScoped<LogUserActivity>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); // add Automapper and tell it where to find mapping profiles

            // services.AddDbContext<DataContext>(options =>
            // {
            //     // options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            //     options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            // });

            // Heroku Connection String Snippet
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
            });


            return services;
        }
    }
}