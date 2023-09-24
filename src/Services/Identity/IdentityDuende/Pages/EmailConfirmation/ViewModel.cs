namespace IdentityDuende.Pages.EmailConfirmation;

public class ViewModel
{
    public ViewModel(bool success)
    {
        Success = success;
    }

    public bool Success { get; set; } = false;
    public string ReturnUrl { get; } = "TESTESTRETURNURL";
}
