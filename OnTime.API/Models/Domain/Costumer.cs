using System;

namespace OnTime.API.Models.Domain;

public class Costumer(IEnumerable<Appointment> appointments)
{
    public IEnumerable<Appointment> Appointments { get; set; } = appointments;
}
