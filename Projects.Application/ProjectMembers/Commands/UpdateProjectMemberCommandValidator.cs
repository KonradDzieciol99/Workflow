using FluentValidation;

namespace Projects.Application.ProjectMembers.Commands
{
    internal class UpdateProjectMemberCommandValidator : AbstractValidator<UpdateProjectMemberCommand>
    {
        public UpdateProjectMemberCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
