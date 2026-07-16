using Microsoft.EntityFrameworkCore;

using OnTime.Domain.Entities;

namespace OnTime.Application.Services;

public interface IAppDbContext
{
    DbSet<Image> Images { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}