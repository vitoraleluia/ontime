using System;

namespace OnTime.API.Models.Domain;

public class Appointment : BaseEntity
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<Session> Services { get; set; }

    private Appointment()
    {
        Services = [];
    }

    public Appointment(DateTime startDate, DateTime endDate, IEnumerable<Session> services)
    {
        StartDate = startDate;
        EndDate = endDate;
        Services = services;
    }
}
