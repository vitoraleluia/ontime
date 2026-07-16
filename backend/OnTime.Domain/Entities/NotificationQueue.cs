using System.ComponentModel.DataAnnotations;

using OnTime.Domain.Common;

namespace OnTime.Domain.Entities;

/// <summary>
/// Represents a persistent message queue in the database.
/// Used to decouple notification sending (email/SMS) from the main request thread.
/// </summary>
public class NotificationQueue : AuditableEntity
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public DateTime ScheduledTime { get; set; }
    public NotificationType NotificationType { get; set; } = NotificationType.Reminder;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public enum NotificationType
{
    Reminder,
    Confirmation,
    Cancellation
}

public enum NotificationStatus
{
    Pending,
    Processing,
    Sent,
    Failed
}