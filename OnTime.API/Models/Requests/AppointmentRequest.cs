using System.ComponentModel.DataAnnotations;

namespace OnTime.API.Models.Requests;

public class AppointmentRequest(DateTime startDate, IEnumerable<ServiceRequest> services)
{
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; } = startDate;
    public IEnumerable<ServiceRequest> Services { get; set; } = services;
}
