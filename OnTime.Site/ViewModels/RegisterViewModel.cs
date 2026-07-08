using System.ComponentModel.DataAnnotations;

namespace OnTime.Site.ViewModels;

public class RegisterViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor forneça o primeiro nome.")]
    public string FirstName { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor forneça o apelido.")]
    public string LastName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Phone(ErrorMessage = "Por favor forneça um número de telemóvel válido.")]
    public string? PhoneNumber { get; set; }
    
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
}