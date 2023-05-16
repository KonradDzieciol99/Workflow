using FluentValidation;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Validators
{
    public class CreateProjectMemberDtoValidator : AbstractValidator<CreateProjectMemberDto>
    {
        public CreateProjectMemberDtoValidator()
        {
            RuleFor(x => x.UserEmail).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.PhotoUrl).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
        }
    }
}
