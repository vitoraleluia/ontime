using System.ComponentModel.DataAnnotations;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class ShopProfessional : AuditableEntity
{
    public int Id { get; set; }

    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    [Required]
    [MaxLength(450)]
    public string ProfessionalId { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation properties for schedules and exceptions
    public List<ProfessionalSchedule> Schedules { get; set; } = [];
    public List<ScheduleException> Exceptions { get; set; } = [];
}