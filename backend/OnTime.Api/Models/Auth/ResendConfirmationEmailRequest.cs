using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Auth;

public class ResendConfirmationEmailRequest
{
    [Required(ErrorMessage = "O endereço de email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O endereço de email introduzido é inválido.")]
    public string Email { get; set; } = string.Empty;
}