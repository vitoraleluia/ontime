namespace OnTime.Site.ViewModels;

public class ConfirmEmailViewModel : BaseViewModel
{
    public bool IsSucceeded { get; set; }
    public string Email { get; set; } = string.Empty;
}
