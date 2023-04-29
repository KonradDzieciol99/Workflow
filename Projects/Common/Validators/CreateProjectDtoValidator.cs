using FluentValidation;
using Projects.Models;
using Projects.Models.Dto;

namespace Projects.Common.Validators
{
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
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
}
