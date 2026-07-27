namespace OnTime.Application.Features.Shops.Responses;

public record ShopResponse
{
    public int Id { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public int SlotDurationMinutes { get; init; }
    public bool AllowCancellation { get; init; }
    public int CancellationDeadlineHours { get; init; }
    public Guid? ImageId { get; init; }
    public string? ImageUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
