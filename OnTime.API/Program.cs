using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using OnTime.API.Database;
using OnTime.API.Models.Domain;
using OnTime.API.Models.Settings;
using OnTime.API.Services.Appointments;
using OnTime.API.Services.Authentication;
using OnTime.API.Services.Sessions;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT Settings
var jwtConfSection = builder.Configuration.GetSection(nameof(JwtSettings));
builder.Services.Configure<JwtSettings>(jwtConfSection);
var jwtSettings = jwtConfSection.Get<JwtSettings>()!;

// Add Authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(b => b.AddPolicy("RequireServiceProvider", policy => policy.RequireRole("ServiceProvider")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllers(opt =>
{
    // Validation errors for SPA apps
    opt.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt => opt.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();