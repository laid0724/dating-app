using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/* 
    here, we are making sure that all of our http methods are throwing errors in the 
    same format (ApiException class in Errors folder) so that it is easier for error 
    reporting in the client side.

    we are achieving this via a middleware, where all http requests goes through the RequestDelegate
    class, injected via the constructor as _next

    We will try the request, and if it fails we will catch the exception, log it in console
    and reformat it into ApiException class and serialize it in json for it to be returned
*/

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // this will pass the http request to the next middleware,
                // if any of the middleware catches an error, it will pass it on to next, and next, and next, etc.
                // we will put this middleware as the first layer so errors derived from http will be caught first.
                await _next(context);
            }
            catch (Exception ex)
            {
                // here, we log the error caught and format it into ApiException

                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error"); // in prod, dont show stack trace for security

                // we want everything to be returned in camelCase, matching JS conventions, so we serialize it as JSON here:

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}