using FluentValidation;
using Projects.Models.Dto;

namespace Projects.Common.Validators
{
    public class CreateProjectMemberDtoValidator : AbstractValidator<CreateProjectMemberDto>
    {
        public CreateProjectMemberDtoValidator()
        {
            RuleFor(x=>x.UserEmail).NotEmpty();
            RuleFor(x=>x.UserId).NotEmpty();
            RuleFor(x=>x.ProjectId).NotEmpty();
            RuleFor(x=>x.PhotoUrl).NotEmpty();
            RuleFor(x=>x.Type).NotEmpty();
        }
    }
}
