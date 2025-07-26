using System;

using OnTime.API.Models.Domain;
using OnTime.API.Models.Requests;

namespace OnTime.API.Extensions;

public static class ServiceExtensions
{
    public static Service ToDomain(this ServiceRequest request)
    {
        return new Service(
            request.Title,
            request.Description,
            request.Duration);
    }
}
