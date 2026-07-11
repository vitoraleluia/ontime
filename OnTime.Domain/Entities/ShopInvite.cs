using System.ComponentModel.DataAnnotations;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

public class ShopInvite : AuditableEntity
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string InvitedEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Token { get; set; } = string.Empty;
    public InviteStatus Status { get; set; } = InviteStatus.Pending;
    public DateTime ExpiresAt { get; set; }
}

public enum InviteStatus
{
    Pending,
    Accepted,
    Expired
}