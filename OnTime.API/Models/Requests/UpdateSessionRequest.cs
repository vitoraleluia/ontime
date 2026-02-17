namespace OnTime.API.Models.Requests;

public record class UpdateSessionRequest(string Title, string Description, int DurationInMinutes);