namespace OnTime.Application.Features.Shops.Responses;

public record SlugAvailabilityResponse
{
    public string Slug { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
    public string Message { get; init; } = string.Empty;
}