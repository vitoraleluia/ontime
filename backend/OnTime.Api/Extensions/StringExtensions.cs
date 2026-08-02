namespace OnTime.Api.Extensions;

public static class StringExtensions
{
    private static string? frontendUrl;

    public static void Configure(string? frontendAddress)
    {
        frontendUrl = frontendAddress;
    }

    public static string BuildFrontendUrl(this string? url)
    {
        if (!string.IsNullOrWhiteSpace(url) &&
            Uri.TryCreate(url, UriKind.Absolute, out var fullUrl) &&
            (fullUrl.Scheme == Uri.UriSchemeHttp || fullUrl.Scheme == Uri.UriSchemeHttps))
        {
            return fullUrl.ToString();
        }

        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            return url ?? "/";
        }

        var path = string.IsNullOrWhiteSpace(url) ? "/" : url;
        return $"{frontendUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }
}