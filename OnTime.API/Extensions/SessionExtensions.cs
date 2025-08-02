using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Extensions;

public static class SessionExtensions
{
    public static Session ToDomain(this CreateSessionRequest request)
    {
        return new Session(
            request.Title,
            request.Description,
            TimeSpan.FromMinutes(request.DurationInMinutes));
    }
}
