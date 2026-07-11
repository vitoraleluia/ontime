namespace OnTime.Identity.Domain.Settings;

public class AuthenticationSettings
{
    public GoogleSettings Google { get; set; } = new();
}

public class GoogleSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}