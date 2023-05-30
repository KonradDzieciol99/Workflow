using FluentValidation;
using Tasks.Application.AppTasks.Commands;

namespace Tasks.Application.AppTasks.Queries;

public class GetAppTaskQueryValidator : AbstractValidator<GetAppTaskQuery>
{
    public GetAppTaskQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
