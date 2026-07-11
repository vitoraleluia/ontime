using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class ScheduleException : AuditableEntity
{
    public int Id { get; set; }

    public int ShopProfessionalId { get; set; }
    public ShopProfessional ShopProfessional { get; set; } = null!;

    public DateOnly Date { get; set; }
    public bool IsOpen { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}