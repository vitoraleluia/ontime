using System;

namespace OnTime.API.Models.Domain;

public class Appointment
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<Service> Services { get; set; }
    public Costumer Costumer { get; set; }

    public Appointment(DateTime startDate, DateTime endDate, IEnumerable<Service> services, Costumer costumer)
    {
        StartDate = startDate;
        EndDate = endDate;
        Services = services;
        Costumer = costumer;
    }
}
