using System.Net;
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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using AutoMapper;

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

            services
              .AddControllers()
              .AddNewtonsoftJson(opt =>
                {
                  opt.SerializerSettings.ReferenceLoopHandling =
                  Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }
              );

            services.AddSwaggerGen(opt =>
              {
                  opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });

                  // ? add authorization header to all API requests made through Swagger
                  opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                  {
                      Description = @"Paste your JWT token here.",
                      Type = SecuritySchemeType.Http,
                      BearerFormat = "JWT",
                      In = ParameterLocation.Header,
                      Scheme = "bearer"
                  });
                  // ? add auth header for [Authorize] endpoints
                  opt.OperationFilter<AddAuthHeaderOperationFilter>();
              }
            );

            services.AddCors(); // ? inject allow CORS service

            services.AddAutoMapper( // ? inject AutoMapper
              typeof(DatingRepository).Assembly // ? tell AutoMapper which assembly to look into
            ); 

            services.AddScoped<IAuthRepository, AuthRepository>(); // ? this makes AuthRepository injectable to other classes.
            services.AddScoped<IDatingRepository, DatingRepository>(); // ? this makes DatingRepository injectable to other classes.

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
            else
            {
                // ? global exception handler middleware for all errors in production mode
                // ! if this is not implemented, server will just return 500 internal server error without any message to the client's console.
                // :: returns 500 status code whenever the server crashes, along with the error message produced by .NET to the client side.
                app.UseExceptionHandler(builder =>
                  builder.Run(async context =>
                  {
                      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // set response status code to 500

                      var error = context.Features.Get<IExceptionHandlerFeature>(); // intercepts exception with handler

                      if (error != null)
                      {
                          // ! return error message as response, but you need to apply CORS for this to show on client side. 
                          context.Response.AddApplicationError(error.Error.Message); // ? applying CORS to this specific response through a custom extension .AddApplicationError()
                          await context.Response.WriteAsync(error.Error.Message);
                      }
                  })
                );
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
