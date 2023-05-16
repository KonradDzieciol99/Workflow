using FluentValidation;
using Projects.Application.Common.Models;

namespace Projects.Application.Validators
{
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
            Uri uriResult;
            bool isUri = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
            return isUri && uriResult.Scheme == Uri.UriSchemeHttps && url.StartsWith("https://1workflowstorage.blob.core.windows.net/projectsicons/");
            //todo pytać api photos
        }

    }
}
