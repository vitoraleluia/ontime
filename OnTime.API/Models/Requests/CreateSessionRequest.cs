using System.ComponentModel.DataAnnotations;

namespace OnTime.API.Models.Requests;

public record CreateSessionRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title must be provided.")]
    string Title,

    string Description,

    [Range(0,TimeSpan.MinutesPerDay,
    ErrorMessage = "The duration must be greater than 0 and less than a day (in minutes).")]
    int DurationInMinutes,

    int OrganizationId);
