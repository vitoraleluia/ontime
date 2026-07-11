using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace OnTime.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
}
