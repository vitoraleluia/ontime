using Microsoft.EntityFrameworkCore;

using OnTime.API.Database;
using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Services.Appointments;

public class AppointmentService : IAppointmentService
{
    private readonly ILogger<AppointmentService> logger;
    private readonly AppDbContext dbContext;

    public AppointmentService(ILogger<AppointmentService> logger, AppDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }

    public async Task<int> Create(CreateAppointmentRequest request, User client)
    {
        var sessions = await dbContext.Sessions
            .AsNoTracking()
            .Where(s => request.SessionIds.Contains(s.Id))
            .ToListAsync();

        if (sessions is null || sessions.Count == 0)
        {
            this.logger.LogWarning("Couldn't find sessions with ids: {@SessionIds}", request.SessionIds);
            return 0;
        }

        if (request.SessionIds.Count() > sessions.Count)
        {
            var missingSessionIds = request.SessionIds.Where(sId => !sessions.Any(s => s.Id == sId));
            this.logger.LogWarning("Missing requested sessions with ids: {@SessionIds}", missingSessionIds);
            return 0;
        }

        var professional = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => request.ProfessionalId.Equals(user.Id));

        if (professional is null)
        {
            this.logger.LogWarning("No professional with id: {ProfessionalId}", request.ProfessionalId);
            return 0;
        }

        var totalDuration = sessions
        .Select(s => s.DurationInMinutes)
        .Aggregate(TimeSpan.Zero, (t1, t2) => t1 + TimeSpan.FromMinutes(t2));

        var endDate = request.StartDate.Add(totalDuration);

        var appointment = new Appointment()
        {
            Client = client,
            EndDate = endDate,
            Professional = professional,
            Services = sessions,
            ClientId = client.Id,
            ProfessionalId = professional.Id,
            StartDate = request.StartDate,

        };

        await dbContext.Appoitnments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();

        return appointment.Id;
    }
}
