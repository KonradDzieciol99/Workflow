using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Projects.Queries
{
    public class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
    {
        public GetProjectsQueryValidator()
        {
            RuleFor(x => x.AppParams).NotNull();
            RuleFor(x => x.AppParams.Take).NotEmpty().GreaterThan(1).LessThan(30);
            RuleFor(x => x.AppParams.Skip);
            RuleFor(x => x.AppParams.OrderBy);
            RuleFor(x => x.AppParams.Filter);
            RuleFor(x => x.AppParams.SelectedColumns);
            RuleFor(x => x.AppParams.GroupBy);
            RuleFor(x => x.AppParams.IsDescending);
            RuleFor(x => x.AppParams.Search);
        }
    }
}
