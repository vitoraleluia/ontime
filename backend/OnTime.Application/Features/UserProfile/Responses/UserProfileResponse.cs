using OnTime.Domain.Enums;

namespace OnTime.Application.Features.UserProfile.Responses;

public class UserProfileResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public UserRole Role { get; set; }
}