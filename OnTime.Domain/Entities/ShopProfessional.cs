using System.ComponentModel.DataAnnotations;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

/// <summary>
/// Represents the assignment of a professional to a specific shop.
/// This acts as a rich join entity that holds the professional's active status,
/// schedules, and exceptions for a particular organization.
/// </summary>
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