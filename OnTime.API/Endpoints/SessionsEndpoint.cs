using Microsoft.AspNetCore.Http.HttpResults;

using OnTime.API.Extensions;

using OnTime.API.Models.Requests;
using OnTime.API.Models.Results;
using OnTime.API.Services.Sessions;

namespace OnTime.API.Endpoints;

public static class SessionsEndpoint
{
    public static void MapSessionsEndpoint(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/sessions");

        group.MapGet("{id}", Get);
        group.MapPost("", Create);
        group.MapDelete("{id}", Delete);
        group.MapPut("{id}", Update);
    }

    private static async Task Get(int id, ISessionService sessionService)
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<Ok<int>, BadRequest, ProblemHttpResult>> Create(
        CreateSessionRequest request,
        ISessionService sessionService)
    {
        var response = await sessionService.Create(request);
        if (response.IsFailure)
        {
            return response.Error.Type switch
            {
                ErrorTypes.Validation => TypedResults.BadRequest(),
                _ => response.Error.ToProblemDetails(),
            };
        }

        return TypedResults.Ok(response.Value);
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