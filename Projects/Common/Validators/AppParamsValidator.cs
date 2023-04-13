using FluentValidation;
using Projects.Models;

namespace Projects.Common.Validators
{
    public class AppParamsValidator : AbstractValidator<AppParams>
    {
        public AppParamsValidator()
        {
            RuleFor(x => x.Take).NotEmpty().InclusiveBetween(1, 30);
            RuleFor(x => x.Skip).NotNull();
            RuleFor(x => x.OrderBy);
            RuleFor(x => x.Filter);
            RuleFor(x => x.SelectedColumns);
            RuleFor(x => x.GroupBy);
            RuleFor(x => x.IsDescending);
            RuleFor(x => x.Search);
        }
    }
}
