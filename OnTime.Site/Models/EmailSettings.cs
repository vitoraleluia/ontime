namespace OnTime.Site.Models;

public class EmailSettings
{
    public string SmtpServer { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 1025;
    public string SenderEmail { get; set; } = "no-reply@ontime.com";
    public string SenderName { get; set; } = "On Time";
}