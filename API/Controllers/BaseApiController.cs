using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // these attributes are no longer needed in other controllers
    // if they inherit this class instead of ControllerBase
    [ServiceFilter(typeof(LogUserActivity))] // apply action filter to all controllers
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {

    }
}