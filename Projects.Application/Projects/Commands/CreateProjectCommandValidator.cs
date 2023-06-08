using FluentValidation;
using Projects.Application.Common.Models.Validators;

namespace Projects.Application.Projects.Commands;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters.");

        RuleFor(x => x.Icon)
            .NotNull()
            .WithMessage("Icon is required.")
            .SetValidator(new IconValidator());
    }
}
