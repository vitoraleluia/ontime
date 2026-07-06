using System.ComponentModel.DataAnnotations;

namespace OnTime.Site.ViewModels;

public class LoginViewModel
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}