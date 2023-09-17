using FluentValidation;

namespace Tasks.Application.AppTasks.Commands;

public class UpdateAppTaskCommandValidator : AbstractValidator<UpdateAppTaskCommand>
{
    public UpdateAppTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).MinimumLength(1).MaximumLength(50).Unless(s => s.Name == null);
        RuleFor(x => x.Priority).IsInEnum().Unless(s => s.Priority == null);
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty().Unless(s => s.StartDate == null);
        RuleFor(x => x.State).NotEmpty().IsInEnum().Unless(s => s.State == null);
        RuleFor(x => x.TaskAssigneeMemberId).MinimumLength(1).MaximumLength(50).Unless(s => s.TaskAssigneeMemberId == null);
        RuleFor(x => x.Description).MaximumLength(500).Unless(s => s.Description == null);
        RuleFor(x => x.DueDate).Must(BeNotMoreThanThreeMonthsFromNow).Unless(x => x.DueDate == null); ;
    }

    private bool BeNotMoreThanThreeMonthsFromNow(DateTime? date)
    {
        var threeMonthsFromNow = DateTime.Now.AddMonths(3);
        return (date <= threeMonthsFromNow);
    }
}
