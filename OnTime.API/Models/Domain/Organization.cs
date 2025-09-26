namespace OnTime.API.Models.Domain;

public class Organization : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<User>? Professionals { get; set; }
    public IEnumerable<Session>? Sessions { get; set; }
}
