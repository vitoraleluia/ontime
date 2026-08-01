namespace OnTime.Application.Models.Identity;

public class IdentityResult
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !this.IsSuccess;
    public bool IsLockedOut { get; init; }
    public bool RequiresTwoFactor { get; init; }
    public string? UserId { get; init; }
    public string? Token { get; init; }
    public string? ErrorMessage { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];

    public static IdentityResult Success(string? userId = null, string? token = null) =>
        new() { IsSuccess = true, UserId = userId, Token = token };

    public static IdentityResult Failure(string error) =>
        new() { IsSuccess = false, ErrorMessage = error, Errors = [error] };

    public static IdentityResult Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return new()
        {
            IsSuccess = false,
            ErrorMessage = errorList.FirstOrDefault(),
            Errors = errorList
        };
    }

    public static IdentityResult LockedOut(string error) =>
        new() { IsSuccess = false, IsLockedOut = true, ErrorMessage = error, Errors = [error] };
}
