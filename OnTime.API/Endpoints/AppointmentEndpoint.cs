using Microsoft.AspNetCore.Http.HttpResults;

using OnTime.API.Models.Requests;
using OnTime.API.Services.Appointment;

namespace OnTime.API.Endpoints;

public static class AppointmentEndpoint
{
    public static void MapAppointmentEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/appointment");

        group.MapPost("", Create)
        .WithName(nameof(Create));

        group.MapDelete("{id}", Cancel)
        .WithName(nameof(Cancel));
    }

    public static Results<Ok<int>, BadRequest> Create(
        CreateAppointmentRequest request,
        IAppointmentService appointmentService)
    {
        var id = appointmentService.Create(request);
        return TypedResults.Ok(id);
    }

    public static IResult Cancel(int id)
    {
        return Results.Ok();
    }
}
