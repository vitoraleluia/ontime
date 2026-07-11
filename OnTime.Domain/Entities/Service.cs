using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class Service : AuditableEntity
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 480, ErrorMessage = "A duração do serviço deve ser entre 1 e 480 minutos.")]
    public int DurationMinutes { get; set; }

    [MaxLength(450)]
    public string ProfessionalId { get; set; } = string.Empty;

    [Range(0.00, 10000.00)]
    [Precision(18, 2)]
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    [MaxLength(200)]
    public string? ImagePath { get; set; }

    // Appointments navigation (many-to-many relationship mapping)
    public List<Appointment> Appointments { get; } = [];
}