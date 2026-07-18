using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace OnTime.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ILogger<BaseApiController> Logger { get; }
    protected IMediator Mediator { get; }

    protected BaseApiController(ILogger<BaseApiController> logger, IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }
}