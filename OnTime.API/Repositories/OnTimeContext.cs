using System;

using Microsoft.EntityFrameworkCore;

using OnTime.API.Models.Domain;

namespace OnTime.API.Repositories;

public class OnTimeContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Appointment> Appoitnments { get; set; }
    public DbSet<Service> MyProperty { get; set; }

    public OnTimeContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;

    }
}
