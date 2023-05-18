using FluentValidation;
using Projects.Application.Projects.Queries;
using Projects.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
