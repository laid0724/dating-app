using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    // an ActionFilter allows us to do something before or after a request has been executed.,
    // similar to interceptors in angular
    // here, we update user's LastActive state using this mechanism.
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); // binding to the event after the http context has been executed

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return; // if user is not logged in, we dont do anything

            var username = resultContext.HttpContext.User.GetUsername(); // get username via ClaimsPrincipal

            var userRepository = resultContext.HttpContext.RequestServices.GetService<IUserRepository>(); // get UserRepository service so we can access its method

            var user = await userRepository.GetUserByUserNameAsync(username); // get app user object via username

            user.LastActive = DateTime.Now;

            await userRepository.SaveAllAsync();
        }
    }
}