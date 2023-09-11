using FluentValidation;

namespace Projects.Application.Projects.Commands;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .Length(1, 40);
    }
}
