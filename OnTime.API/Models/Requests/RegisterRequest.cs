using System.ComponentModel.DataAnnotations;

namespace OnTime.API.Models.Requests;

public record RegisterRequest(
    [Required(ErrorMessage = "First name is required.")]
    [MinLength(2, ErrorMessage = "First name must be at least 2 characters long.")]
    string FirstName,

    [Required(ErrorMessage = "Last name is required.")]
    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters long.")]
    string LastName,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    string Password,

    [Phone(ErrorMessage = "Invalid phone number format.")]
    string? PhoneNumber,

    bool IsProfessional,

    int? OrganizationId
);