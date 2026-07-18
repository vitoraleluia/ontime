using Microsoft.Extensions.Configuration;

namespace OnTime.Application.Common;

public class AppSettings
{
    public static IConfiguration Configuration { get; private set; } = null!;

    public AppSettings(IConfiguration configuration)
    {
        Configuration = configuration;
    }
}