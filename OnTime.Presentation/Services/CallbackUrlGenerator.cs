using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using OnTime.Application.Services;

namespace OnTime.Site.Services;

public class CallbackUrlGenerator : ICallbackUrlGenerator
{
    private readonly LinkGenerator linkGenerator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CallbackUrlGenerator(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        this.linkGenerator = linkGenerator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetConfirmEmailUrl(string userId, string code)
    {
        var httpContext = this.httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return string.Empty;
        }

        return this.linkGenerator.GetUriByAction(
            httpContext,
            action: "ConfirmEmail",
            controller: "Auth",
            values: new { userId, code },
            scheme: httpContext.Request.Scheme,
            host: httpContext.Request.Host) ?? string.Empty;
    }

    public string GetResetPasswordUrl(string code)
    {
        var httpContext = this.httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return string.Empty;
        }

        return this.linkGenerator.GetUriByAction(
            httpContext,
            action: "ResetPassword",
            controller: "Auth",
            values: new { code },
            scheme: httpContext.Request.Scheme,
            host: httpContext.Request.Host) ?? string.Empty;
    }
}