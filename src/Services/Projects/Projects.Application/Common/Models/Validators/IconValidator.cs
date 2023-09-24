using FluentValidation;

namespace Projects.Application.Common.Models.Validators;

public class IconValidator : AbstractValidator<Icon>
{
    public IconValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required.")
            .Must(BeAValidUrl)
            .WithMessage("A valid URL is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters.");
    }

    private bool BeAValidUrl(string url)
    {
        bool isUri = Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult);
        return isUri
            && uriResult is not null
            && uriResult.Scheme == Uri.UriSchemeHttps
            && url.StartsWith("https://1workflowstorage.blob.core.windows.net/projectsicons/");
    }
}
