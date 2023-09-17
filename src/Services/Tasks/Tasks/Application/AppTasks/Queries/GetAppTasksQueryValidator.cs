using FluentValidation;

namespace Tasks.Application.AppTasks.Queries;

public class GetAppTasksQueryValidator : AbstractValidator<GetAppTasksQuery>
{
    public GetAppTasksQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Take).NotNull().GreaterThan(0).LessThan(100);
        RuleFor(x => x.Skip).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.OrderBy);
        RuleFor(x => x.Filter);
        RuleFor(x => x.SelectedColumns);
        RuleFor(x => x.GroupBy);
        RuleFor(x => x.IsDescending);
        RuleFor(x => x.Search);
    }
}
