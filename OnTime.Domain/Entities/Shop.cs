using System.ComponentModel.DataAnnotations;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class Shop : AuditableEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string OwnerId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hífens.")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Range(5, 120, ErrorMessage = "A duração do slot deve ser entre 5 e 120 minutos.")]
    public int SlotDurationMinutes { get; set; } = 30;
    public bool AllowCancellation { get; set; } = true;

    [Range(0, 168, ErrorMessage = "O limite de cancelamento deve ser entre 0 e 168 horas.")]
    public int CancellationDeadlineHours { get; set; } = 24;

    [MaxLength(200)]
    public string? ImagePath { get; set; }

    // Navigation properties
    public List<ShopProfessional> ShopProfessionals { get; } = [];
    public List<Appointment> Appointments { get; } = [];
    public List<ShopInvite> Invites { get; } = [];
}