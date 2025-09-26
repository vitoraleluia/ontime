using Microsoft.AspNetCore.Identity;

namespace OnTime.API.Models.Domain;

public class User : IdentityUser
{
    public bool IsProfessional { get; set; }

    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public IEnumerable<Appointment>? Appointments { get; set; }
}
