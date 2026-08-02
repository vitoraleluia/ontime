using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "O endereço de email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}