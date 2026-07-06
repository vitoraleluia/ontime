using OnTime.API.Models.Requests;

namespace OnTime.API.Services.Appointment;

public interface IAppointmentService
{
    int Create(CreateAppointmentRequest request);
}
