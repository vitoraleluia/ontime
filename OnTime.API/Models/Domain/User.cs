using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace OnTime.API.Models.Domain;

public class User : IdentityUser
{
    [MinLength(3), MaxLength(20)]
    public required string FirstName { get; set; }

    [MinLength(3), MaxLength(20)]
    public required string LastName { get; set; }

    public bool IsProfessional { get; set; }

    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public IEnumerable<Appointment>? Appointments { get; set; }
}
