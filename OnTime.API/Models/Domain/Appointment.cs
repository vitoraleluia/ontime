using System.ComponentModel.DataAnnotations.Schema;

namespace OnTime.API.Models.Domain;

public class Appointment : BaseEntity
{
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required IEnumerable<Session> Services { get; set; }

    public required string ProfessionalId { get; set; }
    [ForeignKey("ProfessionalId")]
    public required User Professional { get; set; }

    public required string ClientId { get; set; }
    [ForeignKey("ClientId")]
    public required User Client { get; set; }
}