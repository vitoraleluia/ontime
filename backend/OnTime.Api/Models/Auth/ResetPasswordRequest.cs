using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Auth;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "O endereço de email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O token é obrigatório.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova palavra-passe é obrigatória.")]
    [MinLength(6, ErrorMessage = "A palavra-passe deve ter pelo menos 6 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;
}
