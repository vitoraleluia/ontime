using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Requests;
using OnTime.API.Services.Appointment;

namespace OnTime.API.Controllers;

public class AppointmentsController : BaseApiController
{
    private readonly IAppointmentService appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        this.appointmentService = appointmentService;
    }

    [HttpPost]
    public ActionResult<int> Create(CreateAppointmentRequest request)
    {
        var id = appointmentService.Create(request);
        return id;
    }

    [HttpDelete]
    public static IResult Cancel(int id)
    {
        return Results.Ok();
    }
}