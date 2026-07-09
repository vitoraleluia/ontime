using System.ComponentModel.DataAnnotations;

namespace OnTime.Site.ViewModels;

public class ForgotPasswordViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Por favor forneça o e-mail.")]
    [EmailAddress(ErrorMessage = "Por favor forneça um e-mail válido.")]
    public string Email { get; set; } = string.Empty;
}
