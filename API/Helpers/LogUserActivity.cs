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
            /* 
                here, we are using the id instead of username because the GetUserByUserNameAsync method
                includes the photo table, which we dont need - so this makes all API calls more efficient
                since this logic is run for every single request.
            */

            var resultContext = await next(); // binding to the event after the http context has been executed

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return; // if user is not logged in, we dont do anything

            var userId = resultContext.HttpContext.User.GetUserId(); // get username via ClaimsPrincipal

            // var username = resultContext.HttpContext.User.GetUsername(); // get username via ClaimsPrincipal

            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>(); // get unit of work service so we can access user repository

            var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId); // get app user object via user id

            // var user = await userRepository.GetUserByUserNameAsync(username); // get app user object via username

            user.LastActive = DateTime.UtcNow;

            await unitOfWork.Complete();
        }
    }
}