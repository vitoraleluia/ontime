namespace OnTime.API.Models.Results;

public record Error(ErrorTypes Type, string Descritpion)
{
    public static Error None = new(ErrorTypes.None, string.Empty);
    public static Error Empty = new(ErrorTypes.Validation, "Must not be empty.");
    public static Error Negative = new(ErrorTypes.Validation, "The value provided must be greater than 0 (zero).");
    public static Error NotFound = new(ErrorTypes.NotFound, "Entity with id not found.");
}

public enum ErrorTypes
{
    None,
    Validation,
    NotFound
}
