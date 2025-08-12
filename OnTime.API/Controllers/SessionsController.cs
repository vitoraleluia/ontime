using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;
using OnTime.API.Services.Sessions;

namespace OnTime.API.Controllers;

public class SessionsController : BaseApiController
{
    private readonly ISessionService sessionService;

    public SessionsController(ISessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SessionResponse>> Get(int id)
    {
        var response = await sessionService.Get(id);

        return response is not null ? response : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateSessionRequest request)
    {
        var id = await sessionService.Create(request);
        return id < 1 ? Problem("Error while creating a session.") : id;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UpdateSessionRequest request)
    {
        if (id < 1)
        {
            ModelState.AddModelError(nameof(id), "Must be positive");
            return BadRequest(ModelState);
        }

        var sucess = await sessionService.Update(id, request);
        return sucess ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (id < 1)
        {
            ModelState.AddModelError(nameof(id), "Must be positive");
            return BadRequest(ModelState);
        }

        var success = await sessionService.Delete(id);
        return success ? NoContent() : NotFound();
    }
}