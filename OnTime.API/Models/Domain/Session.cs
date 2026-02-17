using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnTime.API.Models.Domain;

public class Session : BaseEntity
{
    [MinLength(3), MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public required int DurationInMinutes { get; set; }

    public int OrganizationId { get; set; }
    public required Organization Organization { get; set; }

    public IEnumerable<Appointment>? Appointments { get; set; }
}