using FluentValidation;

namespace Projects.Application.Projects.Queries;

public class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
{
    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.Take).NotEmpty().GreaterThan(1).LessThan(30);
        RuleFor(x => x.Skip);
        RuleFor(x => x.OrderBy);
        RuleFor(x => x.Filter);
        RuleFor(x => x.SelectedColumns);
        RuleFor(x => x.GroupBy);
        RuleFor(x => x.IsDescending);
        RuleFor(x => x.Search);
    }
}
