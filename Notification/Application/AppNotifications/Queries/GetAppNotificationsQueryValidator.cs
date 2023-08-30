using FluentValidation;

namespace Notification.Application.AppNotifications.Queries;

public class GetAppNotificationsQueryValidator : AbstractValidator<GetAppNotificationsQuery>
{
    public GetAppNotificationsQueryValidator()
    {
        RuleFor(x => x.Take).NotEmpty().GreaterThan(0).LessThan(31);
        RuleFor(x => x.Skip);
        RuleFor(x => x.OrderBy);
        RuleFor(x => x.Filter);
        RuleFor(x => x.SelectedColumns);
        RuleFor(x => x.GroupBy);
        RuleFor(x => x.IsDescending);
        RuleFor(x => x.Search);
    }
}
