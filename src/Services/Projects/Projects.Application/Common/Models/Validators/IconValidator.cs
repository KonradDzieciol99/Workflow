﻿using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Projects.Application.Common.Models.Validators;

public class IconValidator : AbstractValidator<Icon>
{
    public IconValidator(IConfiguration configuration)
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
            && url.StartsWith("http://127.0.0.1:10000/devstoreaccount1/projectsicons/");
    }
}
