namespace OnTime.API.Models.Responses;

public record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsProfessional,
    int? OrganizationId,
    string? PhoneNumber
);