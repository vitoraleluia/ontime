using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class Image : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SourceFileName { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public string SourceExtension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}