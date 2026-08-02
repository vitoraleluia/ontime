using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using OnTime.Identity.Entities;

namespace OnTime.Identity.Data;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
    {
    }
}