using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class Appointment : AuditableEntity
{
    public int Id { get; set; }

    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    [Required]
    [MaxLength(450)]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    [MaxLength(450)]
    public string ProfessionalId { get; set; } = string.Empty;

    // Many-to-many relationship: an appointment can consist of multiple services
    public List<Service> Services { get; set; } = [];

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    [Range(0.00, 100000.00)]
    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
}

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}