using OnTime.Domain.Enums;

namespace OnTime.Application.Domain.Results;

public record Error(ErrorCode Code, string Message);