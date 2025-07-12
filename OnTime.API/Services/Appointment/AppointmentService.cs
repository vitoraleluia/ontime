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

    public int Create(AppointmentRequest request)
    {
        var totalDuration = request.Services
        .Select(s => s.Duration)
        .Aggregate(TimeSpan.Zero, (t1, t2) => t1 + t2);

        var endDate = request.StartDate.Add(totalDuration);

        var appointment = request.ToDomain(endDate);

        return 0;
    }
}
