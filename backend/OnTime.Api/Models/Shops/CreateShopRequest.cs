using System.ComponentModel.DataAnnotations;

namespace OnTime.Api.Models.Shops;

public class CreateShopRequest
{
    [Required(ErrorMessage = "O nome do estabelecimento é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome do estabelecimento não pode exceder 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "O slug não pode exceder 50 caracteres.")]
    [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hífens.")]
    public string? Slug { get; set; }

    [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "O endereço não pode exceder 200 caracteres.")]
    public string? Address { get; set; }

    [MaxLength(20, ErrorMessage = "O número de telefone não pode exceder 20 caracteres.")]
    [RegularExpression(@"^(\+351|00351)?\s?[29](\s?\d){8}$", ErrorMessage = "O número de telefone deve ser um número nacional português válido (ex: 272 123 123 ou 912 345 678).")]
    public string? PhoneNumber { get; set; }

    [Range(5, 120, ErrorMessage = "A duração da marcação deve ser entre 5 e 120 minutos.")]
    public int SlotDurationMinutes { get; set; } = 30;

    public bool AllowCancellation { get; set; } = true;

    [Range(0, 168, ErrorMessage = "O limite para cancelamento deve ser entre 0 e 168 horas.")]
    public int CancellationDeadlineHours { get; set; } = 24;

    public Guid? ImageId { get; set; }
}