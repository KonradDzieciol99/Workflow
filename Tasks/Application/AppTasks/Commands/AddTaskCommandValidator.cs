using FluentValidation;

namespace Tasks.Application.AppTasks.Commands;

public class AddTaskCommandValidator : AbstractValidator<AddTaskCommand>
{
    public AddTaskCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(1).MaximumLength(50);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.State).IsInEnum();
        //RuleFor(x => x.TaskAssigneeMemberEmail)
        //    .EmailAddress();
        //.When(x => !string.IsNullOrEmpty(x.TaskAssigneeMemberEmail)); TEST !
        RuleFor(x => x.TaskAssigneeMemberId).MinimumLength(1).MaximumLength(50);
        //RuleFor(x => x.TaskAssigneeMemberPhotoUrl).MinimumLength(1).MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DueDate);
    }
}