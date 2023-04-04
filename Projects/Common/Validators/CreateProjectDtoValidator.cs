using FluentValidation;
using Projects.Models;
using Projects.Models.Dto;

namespace Projects.Common.Validators
{
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(20).MinimumLength(6);
        }
    }
}
