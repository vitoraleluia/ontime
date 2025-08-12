using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using OnTime.API.Models.Domain;

namespace OnTime.API.Database;

public class UserDbContext : IdentityDbContext<User>
{
    public UserDbContext(DbContextOptions options) : base(options)
    {
    }

    protected UserDbContext()
    {
    }
}
