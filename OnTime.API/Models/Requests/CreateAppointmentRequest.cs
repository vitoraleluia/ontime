
namespace OnTime.API.Models.Requests;

public record CreateAppointmentRequest(
    DateTime StartDate,
    IEnumerable<int> SessionIds,
    string ProfessionalId);