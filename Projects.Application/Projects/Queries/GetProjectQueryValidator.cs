using FluentValidation;

namespace Projects.Application.Projects.Queries;

public class GetProjectQueryValidator : AbstractValidator<GetProjectQuery>
{
    public GetProjectQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
