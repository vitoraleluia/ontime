using System.ComponentModel.DataAnnotations;

namespace OnTime.Site.ViewModels;

public class ResetPasswordViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Por favor forneça o e-mail.")]
    [EmailAddress(ErrorMessage = "Por favor forneça um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Por favor forneça a palavra-passe.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Por favor confirme a palavra-passe.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "As passwords não coincidem.")]
    public string PasswordConfirmation { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
