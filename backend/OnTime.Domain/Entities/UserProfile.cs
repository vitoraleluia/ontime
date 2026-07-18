using System.ComponentModel.DataAnnotations;

using OnTime.Domain.Common;
using OnTime.Domain.Enums;

namespace OnTime.Domain.Entities;

public class UserProfile : AuditableEntity
{
    [Key]
    [MaxLength(450)]
    public string Id { get; set; } = string.Empty; // Maps to Keycloak sub claim

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    public Guid? ProfilePictureId { get; set; }
    public Image? ProfilePicture { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.Client;
}