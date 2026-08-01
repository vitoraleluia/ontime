using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "O endereço de email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
    [MinLength(6, ErrorMessage = "A palavra-passe deve ter pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "O primeiro nome é obrigatório.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O apelido é obrigatório.")]
    public string LastName { get; set; } = string.Empty;

    [RegularExpression(@"^(\+351)?9\d{8}$", ErrorMessage = "O número de telemóvel deve ser um número português válido (ex: 927431783).")]
    public string? PhoneNumber { get; set; }
}
