using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public enum ImageFormat
{
    Square,
    Landscape,
    Portrait
}

public enum ImageStatus
{
    Pending,
    Processing,
    Processed,
    Failed
}

public class Image : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SourceFileName { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public string SourceExtension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public ImageFormat Format { get; set; } = ImageFormat.Square;
    public ImageStatus Status { get; set; } = ImageStatus.Pending;
    public string? ProcessedPath { get; set; }
    public string? ErrorMessage { get; set; }
}