using System;

using Microsoft.AspNetCore.Http.HttpResults;

using OnTime.API.Models.Results;

namespace OnTime.API.Extensions;

public static class ErrorExtensions
{
    public static ProblemHttpResult ToProblemDetails(this Error error)
    {
        return TypedResults.Problem(
            title: error.Type.ToString(),
            detail: error.Descritpion,
            statusCode: 500
        );
    }

}
