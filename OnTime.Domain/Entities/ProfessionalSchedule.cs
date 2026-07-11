using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class ProfessionalSchedule : AuditableEntity
{
    public int Id { get; set; }

    public int ShopProfessionalId { get; set; }
    public ShopProfessional ShopProfessional { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}