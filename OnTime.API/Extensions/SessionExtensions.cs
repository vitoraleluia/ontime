using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;
using OnTime.API.Models.Responses;

namespace OnTime.API.Extensions;

public static class SessionExtensions
{
    public static SessionResponse ToResponse(this Session domain)
    {
        return new SessionResponse(
            domain.Title,
            domain.Description,
            domain.DurationInMinutes);
    }

    public static Session Update(this Session session, UpdateSessionRequest request)
    {
        session.Title = request.Title;
        session.Description = request.Description;
        session.DurationInMinutes = request.DurationInMinutes;
        return session;
    }
}