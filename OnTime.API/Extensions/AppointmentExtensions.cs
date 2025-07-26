using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Extensions;

public static class AppointmentExtensions
{
    public static Appointment ToDomain(this AppointmentRequest request, DateTime endDate)
    {
        return new Appointment(
            request.StartDate,
            endDate,
            request.Services.Select(s => s.ToDomain()),
            null);
    }
}
