using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Auth;

public class ConfirmEmailRequest
{
    [Required(ErrorMessage = "O ID de utilizador é obrigatório.")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "O token é obrigatório.")]
    public string Token { get; set; } = string.Empty;
}