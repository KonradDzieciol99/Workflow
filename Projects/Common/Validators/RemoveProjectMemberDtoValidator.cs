using FluentValidation;
using Projects.Models.Dto;

namespace Projects.Common.Validators
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
