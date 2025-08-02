using System;

using Microsoft.EntityFrameworkCore;

using OnTime.API.Models.Domain;

namespace OnTime.API.Repositories;

public class OnTimeContext : DbContext
{
    public DbSet<Appointment> Appoitnments { get; set; }
    public DbSet<Session> Services { get; set; }

    public OnTimeContext(DbContextOptions options) : base(options)
    {
    }
}
