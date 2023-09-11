
namespace IdentityDuende.Pages.Account.Register;

public class RegisterViewModel
{
    public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
    public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
    public string? ExternalLoginScheme => ExternalProviders?.SingleOrDefault()?.AuthenticationScheme;
    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
