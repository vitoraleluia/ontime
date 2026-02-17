using System.ComponentModel.DataAnnotations;

namespace OnTime.API.Models.Requests;

public record LoginRequest(
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    string Password
);