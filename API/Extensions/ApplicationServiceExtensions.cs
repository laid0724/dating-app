using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
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
            // lifetime: how long should the service be alive for once it is created. 3 options:
            // 1. singleton: created once and lasts through he lifetime of the application, until application is shut down
            // 2. scoped: scoped to the lifetime of the http request - when a requests comes in and we have this service injected into that particular controller, 
            //      then a new instance of this service is created, and when request finishes it is disposed.
            // 3. transient: created and destroyed as soon as the method is finished

            // most appropriate for http requests and for generating token, used most of the time for web apis.
            // provide interface and implementation:
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); // add Automapper and tell it where to find mapping profiles

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}