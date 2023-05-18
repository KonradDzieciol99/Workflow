using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.ProjectMembers.Commands
{
    internal class UpdateProjectMemberCommandValidator: AbstractValidator<UpdateProjectMemberCommand>
    {
        public UpdateProjectMemberCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
