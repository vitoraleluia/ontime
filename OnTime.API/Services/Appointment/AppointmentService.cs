using OnTime.API.Extensions;
using OnTime.API.Models.Requests;

namespace OnTime.API.Services.Appointment;

public class AppointmentService : IAppointmentService
{
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(ILogger<AppointmentService> logger)
    {
        _logger = logger;
    }

    public int Create(CreateAppointmentRequest request)
    {
        var totalDuration = request.Sessions
        .Select(s => s.DurationInMinutes)
        .Aggregate(TimeSpan.Zero, (t1, t2) => t1 + TimeSpan.FromMinutes(t2));

        var endDate = request.StartDate.Add(totalDuration);

        var appointment = request.ToDomain(endDate);

        return 0;
    }
}
