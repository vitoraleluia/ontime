using Microsoft.EntityFrameworkCore;
using OnTime.Application.DependencyInjection;
using OnTime.Identity.DependencyInjection;
using OnTime.Infrastructure.DependencyInjection;
using OnTime.Site.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Database & Identity services from the Identity project
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddPresentationServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var identityContext = scope.ServiceProvider.GetRequiredService<OnTime.Identity.Data.ApplicationDbContext>();
    await identityContext.Database.MigrateAsync();

    var domainContext = scope.ServiceProvider.GetRequiredService<OnTime.Infrastructure.Data.OnTimeDbContext>();
    await domainContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();