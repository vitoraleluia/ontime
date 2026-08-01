using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using OnTime.Api.DependencyInjection;
using OnTime.Application.DependencyInjection;
using OnTime.Application.Domain.Settings;
using OnTime.Identity.Constants;
using OnTime.Identity.Data;
using OnTime.Identity.DependencyInjection;
using OnTime.Identity.Entities;
using OnTime.Infrastructure.Data;
using OnTime.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Database & Identity services
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// Register AppSettings for static configuration access
builder.Services.AddSingleton(new AppSettings(builder.Configuration));

var app = builder.Build();

// Automatically apply migrations and seed Identity roles on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        await identityContext.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = [IdentityRoles.Client, IdentityRoles.Professional];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var appContext = services.GetRequiredService<AppDbContext>();
        await appContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar migrações ou semear papéis de identidade.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map Identity API endpoints under /api/auth
app.MapGroup("/api/auth").MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();