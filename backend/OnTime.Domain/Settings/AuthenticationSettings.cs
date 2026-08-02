namespace OnTime.Domain.Settings;

public class AuthenticationSettings
{
    public string ClientUrl { get; set; } = string.Empty;
    public GoogleSettings Google { get; set; } = new();
    public LockoutSettings Lockout { get; set; } = new();
}

public class GoogleSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class LockoutSettings
{
    public int DefaultLockoutTimeSpanInMinutes { get; set; } = 5;
    public int MaxFailedAccessAttempts { get; set; } = 5;
}