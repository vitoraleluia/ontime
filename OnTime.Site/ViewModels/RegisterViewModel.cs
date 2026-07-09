using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace OnTime.Site.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor forneça o primeiro nome.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O primeiro nome deve conter apenas letras.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor forneça o apelido.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O apelido deve conter apenas letras.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Por favor forneça o e-mail.")]
    [EmailAddress(ErrorMessage = "Por favor forneça um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Por favor forneça um número de telemóvel válido.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Por favor forneça a palavra-passe.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Por favor confirme a palavra-passe.")]
    public string PasswordConfirmation { get; set; } = string.Empty;
}