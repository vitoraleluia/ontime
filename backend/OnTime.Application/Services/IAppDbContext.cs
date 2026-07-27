using Microsoft.EntityFrameworkCore;

using OnTime.Domain.Entities;

namespace OnTime.Application.Services;

public interface IAppDbContext
{
    DbSet<Shop> Shops { get; set; }
    DbSet<ShopProfessional> ShopProfessionals { get; set; }
    DbSet<Image> Images { get; set; }
    DbSet<UserProfile> UserProfiles { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}