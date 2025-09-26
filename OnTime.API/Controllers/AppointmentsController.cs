using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;
using OnTime.API.Services.Appointments;

namespace OnTime.API.Controllers;

[Authorize]
public class AppointmentsController(
    IAppointmentService appointmentService,
    UserManager<User> userManager) : BaseApiController
{
    private readonly IAppointmentService appointmentService = appointmentService;

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateAppointmentRequest request)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user is null)
            return Unauthorized();

        var id = await appointmentService.Create(request, user);
        return id > 0 ? id : Problem();
    }

    [HttpDelete]
    public IResult Cancel(int id)
    {
        return Results.Ok();
    }
}