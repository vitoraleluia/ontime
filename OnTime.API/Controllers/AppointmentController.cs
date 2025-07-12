using Microsoft.AspNetCore.Mvc;

using OnTime.API.Models.Requests;

namespace OnTime.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(ILogger<AppointmentController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "CreateNewAppointment")]
    public IActionResult Create(AppointmentRequest request)
    {
        return Ok();
    }
}
