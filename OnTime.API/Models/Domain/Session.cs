namespace OnTime.API.Models.Domain;

public class Session : BaseEntity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required int DurationInMinutes { get; set; }

    public int OrganizationId { get; set; }
    public required Organization Organization { get; set; }
    public IEnumerable<Appointment>? Appointments { get; set; }
}
