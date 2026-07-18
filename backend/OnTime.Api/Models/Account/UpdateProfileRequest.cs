using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Account;

public record UpdateProfileRequest(
    [Required(ErrorMessage = "O primeiro nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O primeiro nome não pode exceder 100 caracteres.")]
    string FirstName,

    [Required(ErrorMessage = "O apelido é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O apelido não pode exceder 100 caracteres.")]
    string LastName,

    [RegularExpression(@"^(?:\+351|00351)?(?:9\d{8}|2\d{8})$", ErrorMessage = "O número de telemóvel deve ser um número português válido.")]
    string? PhoneNumber,

    Guid? ProfilePictureId
);