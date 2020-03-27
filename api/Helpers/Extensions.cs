using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(
          this HttpResponse response, // ? 'this' is the parent object this method is being applied to
          string message // ? argument to pass into this method
        )
        {
            // ! note that the spelling has to be exact or the client-side wont be able to interpret these in the header response!
            response.Headers.Add("Application-Error", message); // ? adds message as Application-Error to the headers
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error"); // ? exposes "Application-Error" to the headers
            response.Headers.Add("Access-Control-Allow-Origin", "*"); // ? allows origin (CORS), * means any origin
        }
    }
}