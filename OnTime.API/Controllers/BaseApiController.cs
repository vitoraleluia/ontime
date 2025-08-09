using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnTime.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
