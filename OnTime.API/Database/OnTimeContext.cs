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

    public override int SaveChanges()
    {
        AddTimestaps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestaps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestaps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
        }
    }
}
