using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Projects.Queries
{
    public class GetProjectQueryValidatord : AbstractValidator<GetProjectQuery>
    {
        public GetProjectQueryValidatord()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
