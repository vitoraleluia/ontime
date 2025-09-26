using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Services.Appointments;

public interface IAppointmentService
{
    Task<int> Create(CreateAppointmentRequest request, User client);
}
