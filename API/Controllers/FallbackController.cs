using System.IO;
using Microsoft.AspNetCore.Mvc;

// IMPORTANT FOR HOSTING ANGULAR APP
// here, we are not deriving from base api because this is a controller with MVC support,
// used to configure our view routes, which in our case is our angular app
namespace API.Controllers
{
    public class FallbackController : ControllerBase
    {
        public ActionResult Index()
        {
            return PhysicalFile(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "index.html"
                ),
                "text/HTML"
            );
        }
    }
}