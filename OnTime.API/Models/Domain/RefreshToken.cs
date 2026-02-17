using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnTime.API.Models.Domain;

public class RefreshToken : BaseEntity
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    public DateTime ExpiresAt { get; set; }
}