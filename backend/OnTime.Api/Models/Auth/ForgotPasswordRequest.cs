using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "O endereço de email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; } = string.Empty;
}