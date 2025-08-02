using Microsoft.EntityFrameworkCore;

using OnTime.API.Endpoints;

using OnTime.API.Repositories;
using OnTime.API.Services.Appointment;
using OnTime.API.Services.Session;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<OnTimeContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
    });
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapAppointmentEndpoint();
app.MapSessionsEndpoint();

app.Run();
