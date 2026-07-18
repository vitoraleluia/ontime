using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnTime.Api.Controllers;

public class AuthTestsController : BaseApiController
{
    public AuthTestsController(ILogger<BaseApiController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet("/no-login")]
    public IActionResult NoLogin(string name) => Ok($"Hello, {name}!");

    [Authorize]
    [HttpGet("/with-login")]
    public IActionResult WithLogin(string name) => Ok($"Hello, {name}!");
}