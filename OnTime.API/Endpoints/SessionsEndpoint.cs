using Microsoft.AspNetCore.Http.HttpResults;

using OnTime.API.Models.Requests;
using OnTime.API.Services.Session;

namespace OnTime.API.Endpoints;

public static class SessionsEndpoint
{
    public static void MapSessionsEndpoint(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/enpoints");

        group.MapGet("{id}", Get);
        group.MapPost("", Create);
        group.MapDelete("{id}", Delete);
        group.MapPut("{id}", Update);
    }

    private static async Task Get(int id, ISessionService sessionService)
    {
        throw new NotImplementedException();
    }

    private static Results<Ok<int>, BadRequest> Create(
        CreateSessionRequest request,
        ISessionService sessionService)
    {
        var id = sessionService.Create(request);
        return TypedResults.Ok(id);
    }

    private static async Task Update(int id, ISessionService sessionService)
    {
        throw new NotImplementedException();
    }

    private static async Task Delete(int id, ISessionService sessionService)
    {
        throw new NotImplementedException();
    }
}