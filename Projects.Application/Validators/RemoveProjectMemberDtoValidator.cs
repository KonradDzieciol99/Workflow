using FluentValidation;
using Projects.Application.Common.Models.Dto;

namespace Projects.Application.Validators
{
    public class RemoveProjectMemberDtoValidator : AbstractValidator<RemoveProjectMemberDto>
    {
        public RemoveProjectMemberDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
