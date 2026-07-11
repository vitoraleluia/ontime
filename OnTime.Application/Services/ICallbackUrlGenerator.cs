namespace OnTime.Application.Services;

public interface ICallbackUrlGenerator
{
    string GetConfirmEmailUrl(string userId, string code);
    string GetResetPasswordUrl(string code);
}