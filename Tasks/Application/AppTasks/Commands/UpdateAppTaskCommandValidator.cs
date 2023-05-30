using FluentValidation;

namespace Tasks.Application.AppTasks.Commands;

public class UpdateAppTaskCommandValidator : AbstractValidator<UpdateAppTaskCommand>
{
    public UpdateAppTaskCommandValidator()
    {
        RuleFor(x => x.AppTaskDto.Id).NotEmpty();
        RuleFor(x => x.AppTaskDto.Name).NotEmpty().MinimumLength(1).MaximumLength(50);
        RuleFor(x => x.AppTaskDto.Priority).IsInEnum();
        RuleFor(x => x.AppTaskDto.ProjectId).NotEmpty();
        RuleFor(x => x.AppTaskDto.StartDate).NotEmpty();
        RuleFor(x => x.AppTaskDto.State).NotEmpty().IsInEnum();
        RuleFor(x => x.AppTaskDto.TaskAssigneeMemberEmail)
            .EmailAddress();
        //.When(x => !string.IsNullOrEmpty(x.TaskAssigneeMemberEmail)); TEST !
        RuleFor(x => x.AppTaskDto.TaskAssigneeMemberId).MinimumLength(1).MaximumLength(50);
        RuleFor(x => x.AppTaskDto.TaskAssigneeMemberPhotoUrl).MinimumLength(1).MaximumLength(150);
        RuleFor(x => x.AppTaskDto.Description).MaximumLength(500);
        RuleFor(x => x.AppTaskDto.DueDate);
    }
}
