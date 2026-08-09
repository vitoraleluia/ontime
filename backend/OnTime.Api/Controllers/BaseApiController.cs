using MediatR;

using Microsoft.AspNetCore.Mvc;

using OnTime.Application.Domain.Results;
using OnTime.Domain.Common;
using OnTime.Domain.Enums;

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

    protected ObjectResult HandleFailure(Error error)
    {
        var statusCode = error.Code switch
        {
            ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorCode.Forbidden or ErrorCode.ProfessionalRoleRequired => StatusCodes.Status403Forbidden,
            ErrorCode.NotFound or ErrorCode.ProfileNotFound or ErrorCode.ShopNotFound or ErrorCode.ImageNotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new ErrorResponse(error.Code, error.Message));
    }

    protected ObjectResult HandleFailure(ErrorCode code, string message, int statusCode = StatusCodes.Status400BadRequest)
    {
        return StatusCode(statusCode, new ErrorResponse(code, message));
    }
}