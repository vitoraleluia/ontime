namespace OnTime.API.Models.Requests;

public record CreateSessionRequest(string Title, string Description, int DurationInMinutes);
