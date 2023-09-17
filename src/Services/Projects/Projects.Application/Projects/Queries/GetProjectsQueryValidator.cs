using FluentValidation;

namespace Projects.Application.Projects.Queries;

public class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
{
    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.Take).NotEmpty().GreaterThan(0).LessThan(100);
        RuleFor(x => x.Skip).NotEmpty().GreaterThanOrEqualTo(0);
        RuleFor(x => x.OrderBy);
        RuleFor(x => x.Filter);
        RuleFor(x => x.SelectedColumns);
        RuleFor(x => x.GroupBy);
        RuleFor(x => x.IsDescending);
        RuleFor(x => x.Search);
    }
}
