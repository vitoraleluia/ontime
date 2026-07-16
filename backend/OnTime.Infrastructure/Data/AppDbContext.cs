using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using OnTime.Application.Services;
using OnTime.Domain.Common;
using OnTime.Domain.Entities;

namespace OnTime.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Shop> Shops { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ShopProfessional> ShopProfessionals { get; set; }
    public DbSet<ProfessionalSchedule> ProfessionalSchedules { get; set; }
    public DbSet<ScheduleException> ScheduleExceptions { get; set; }
    public DbSet<ShopInvite> ShopInvites { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<NotificationQueue> NotificationQueue { get; set; }
    public DbSet<Image> Images { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userName = this.httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        var entries = this.ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var auditableEntity = (AuditableEntity)entityEntry.Entity;
            var now = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                auditableEntity.CreatedAt = now;
                auditableEntity.CreatedBy = userName;
            }

            auditableEntity.UpdatedAt = now;
            auditableEntity.UpdatedBy = userName;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}