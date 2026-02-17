using System.ComponentModel.DataAnnotations;

namespace OnTime.API.Models.Domain;

public class Organization : BaseEntity
{
    [MinLength(3), MaxLength(100)]
    public required string Name { get; set; }
    public IEnumerable<User>? Professionals { get; set; }
    public IEnumerable<Session>? Sessions { get; set; }
}