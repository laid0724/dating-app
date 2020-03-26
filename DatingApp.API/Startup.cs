using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DatingApp.API
{
    public class Startup
    {
        // ? Startup constructor, dependency injection
        public Startup(IConfiguration configuration)
        {
            // ? this injects configuration settings from appsettings.json
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ? this is for registering injectable services (DI), i.e., services that can be injected in constructor in Angular
        public void ConfigureServices(IServiceCollection services)
        // * ordering does not matter for services; only for middlewares in Configure()
        {
            // ? Injects DataContext to connect to a db
            services.AddDbContext<DataContext>(
                // need to install sqlite entity package for this
                // ! connection string should be stored in appsettings.json
                // data => data.UseSqlite("ConnectionString")
                data => data.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")
                  )
                // then, you need to create a migration with the entity framework tools
                // ! need to install dotnet-ef tool and package: Microsoft.EntityFrameworkCore.Design
                // * `dotnet ef migrations add InitialCreate`
                // * to apply migrations, run `dotnet ef database update`

            );

            services.AddControllers();

            services.AddSwaggerGen(c =>
              {
                  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dating App API", Version = "v1" });
              }
            );

            services.AddCors(); // ? inject allow CORS service

            services.AddScoped<IAuthRepository, AuthRepository>(); // ? this makes AuthRepository injectable to other classes.

            // ? this adds authentication service so that controllers with the [Authorize] attribute know what to do to authenticate the requests
            services
                .AddAuthentication(
                  JwtBearerDefaults.AuthenticationScheme  // ? Authenticate via JWT
                )
                .AddJwtBearer(
                  options => options.TokenValidationParameters = new TokenValidationParameters
                  {
                      // ? here, we specify the options we want to authenticate against
                      ValidateIssuerSigningKey = true, // ? tell options to validate key
                      IssuerSigningKey = new SymmetricSecurityKey( // ? get key from env file
                      Encoding.ASCII.GetBytes(
                        Configuration.GetSection("AppSettings:Token").Value
                      )
                    ),
                      ValidateIssuer = false, // ? right now it's local host, so set to false
                      ValidateAudience = false, // ? right now it's local host, so set to false
                  }
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ? this is for configuring the http request pipeline; everything added here are middlewares.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                  // To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                  // c.RoutePrefix = string.Empty;
              }
            );

            // ? this will redirect all http requests to https requests.
            // app.UseHttpsRedirection(); 

            app.UseRouting();

            // ? set CORS policy with middleware; this needs to go after UseRouting and before UseAuth & Use Endpoints:
            app.UseCors(c => c
                // .WithOrigins() // ? you can specify origin, but for development purposes, allow all:
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            // ? register the injected authentication service as a middleware; has to be before .UseAuthorization()!
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
