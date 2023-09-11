using FluentValidation;

namespace Tasks.Application.AppTasks.Queries;

public class GetAppTaskQueryValidator : AbstractValidator<GetAppTaskQuery>
{
    public GetAppTaskQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
