using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Extensions;

public static class AppointmentExtensions
{
    public static Appointment ToDomain(this CreateAppointmentRequest request, DateTime endDate)
    {
        return new Appointment(
            request.StartDate,
            endDate,
            request.Sessions.Select(s => s.ToDomain()),
            null);
    }
}
