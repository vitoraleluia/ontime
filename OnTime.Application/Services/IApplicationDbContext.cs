using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnTime.Domain.Entities;

namespace OnTime.Application.Services;

public interface IApplicationDbContext
{
    DbSet<Image> Images { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
