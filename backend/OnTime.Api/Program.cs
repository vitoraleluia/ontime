using Microsoft.EntityFrameworkCore;

using OnTime.Api.DependencyInjection;
using OnTime.Application.DependencyInjection;
using OnTime.Infrastructure.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add Database & Identity services from the Identity project
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId(builder.Configuration["Authentication:ClientId"]);
        options.OAuthUsePkce();
    });
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
app.MapFallbackToFile("index.html");

app.MapControllers();

app.Run();