using OnTime.Application.DependencyInjection;
using OnTime.Application.Services;
using OnTime.Identity.DependencyInjection;
using OnTime.Infrastructure.DependencyInjection;
using OnTime.Site.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Database & Identity services from the Identity project
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddTransient<IServerSideRenderer, ServerSideRenderer>();
builder.Services.AddScoped<ICallbackUrlGenerator, CallbackUrlGenerator>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

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