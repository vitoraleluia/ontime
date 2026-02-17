using OnTime.API.Database;
using OnTime.API.Extensions;
using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;

namespace OnTime.API.Services.Sessions;

class SessionService : ISessionService
{
    private readonly ILogger<SessionService> logger;
    private readonly AppDbContext dbContext;

    public SessionService(ILogger<SessionService> logger, AppDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;

    }

    public async Task<int> Create(CreateSessionRequest request)
    {
        var org = await this.dbContext.Organizations.FindAsync(request.OrganizationId);
        if (org is null)
            return 0;

        var newSession = new Session()
        {
            DurationInMinutes = request.DurationInMinutes,
            Organization = org,
            Title = request.Title
        };

        this.dbContext.Sessions.Add(newSession);
        await this.dbContext.SaveChangesAsync();

        return newSession.Id;
    }

    public async Task<bool> Delete(int id)
    {
        var session = await this.dbContext.Sessions.FindAsync(id);
        if (session is null)
            return false;

        this.dbContext.Sessions.Remove(session);
        await this.dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<SessionResponse?> Get(int id)
    {
        var session = await this.dbContext.Sessions.FindAsync(id);
        return session?.ToResponse();
    }

    public async Task<bool> Update(int id, UpdateSessionRequest request)
    {
        var session = await this.dbContext.Sessions.FindAsync(id);
        if (session is null)
            return false;

        session.Update(request);
        await this.dbContext.SaveChangesAsync();

        return true;
    }
}