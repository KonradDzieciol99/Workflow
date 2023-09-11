using FluentValidation;

namespace Tasks.Application.AppTasks.Commands;

public class DeleteAppTaskCommandValidator : AbstractValidator<DeleteAppTaskCommand>
{
    public DeleteAppTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
