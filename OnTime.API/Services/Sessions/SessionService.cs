using OnTime.API.Database;
using OnTime.API.Extensions;
using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;

namespace OnTime.API.Services.Sessions;

class SessionService : ISessionService
{
    private readonly ILogger<SessionService> logger;
    private readonly OnTimeContext dbContext;

    public SessionService(ILogger<SessionService> logger, OnTimeContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;

    }

    public async Task<int> Create(CreateSessionRequest request)
    {
        var session = request.ToDomain();

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

        return session.Id;
    }

    public async Task<bool> Delete(int id)
    {
        var session = await dbContext.Sessions.FindAsync(id);
        if (session is null)
            return false;

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<SessionResponse?> Get(int id)
    {
        var session = await dbContext.Sessions.FindAsync(id);
        return session?.ToResponse();
    }

    public async Task<bool> Update(int id, UpdateSessionRequest request)
    {
        var session = await dbContext.Sessions.FindAsync(id);
        if (session is null)
            return false;

        session.Update(request);
        await dbContext.SaveChangesAsync();

        return true;
    }
}