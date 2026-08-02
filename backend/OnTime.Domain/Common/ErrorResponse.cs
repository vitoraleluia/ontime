using OnTime.Domain.Enums;

namespace OnTime.Domain.Common;

public class ErrorResponse
{
    public ErrorCode ErrorCode { get; set; }
    public string Message { get; set; } = string.Empty;

    public ErrorResponse() { }

    public ErrorResponse(ErrorCode errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
    }
}
