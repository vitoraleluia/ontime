using Microsoft.EntityFrameworkCore;

using OnTime.API.Models.Domain;

namespace OnTime.API.Database;

public class OnTimeContext : DbContext
{
    public DbSet<Appointment> Appoitnments { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public OnTimeContext(DbContextOptions options) : base(options)
    {
    }
}
