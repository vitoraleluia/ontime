namespace OnTime.Application.Domain.Settings;

public class ImageStorageSettings
{
    public string RawFolder { get; set; } = "uploads/raw";
    public string ProcessedFolder { get; set; } = "media";
}
