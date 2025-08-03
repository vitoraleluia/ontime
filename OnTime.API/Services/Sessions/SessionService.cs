using OnTime.API.Database;
using OnTime.API.Extensions;
using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;
using OnTime.API.Models.Results;

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

    public async Task<Result<int>> Create(CreateSessionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Result.Failure<int>(Error.Empty);

        if (request.DurationInMinutes < 1)
            return Result.Failure<int>(Error.Negative);

        var session = request.ToDomain();

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

        return Result.Success(session.Id);
    }

    public async Task<Result> Delete(int id)
    {
        if (id < 1)
            return Result.Failure(Error.Negative);

        var session = await dbContext.Sessions.FindAsync(id);
        if (session is null)
            return Result.Failure(Error.NotFound);

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<SessionResponse>> Get(int id)
    {
        if (id < 1)
            return Result.Failure<SessionResponse>(Error.Negative);

        var session = await dbContext.Sessions.FindAsync(id);
        if (session is null)
            return Result.Failure<SessionResponse>(Error.NotFound);

        return Result.Success(session.ToResponse());
    }

    public async Task<Result> Update(int id, UpdateSessionRequest request)
    {
        if (id < 1)
            return Result.Failure<SessionResponse>(Error.Negative);

        var session = await dbContext.Sessions.FindAsync(id);
        if (session is null)
            return Result.Failure<SessionResponse>(Error.NotFound);

        session.Update(request);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }
}