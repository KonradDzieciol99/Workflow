using FluentValidation;

namespace Notification.Application.AppNotifications.Queries;

public class GetAppNotificationsQueryValidator : AbstractValidator<GetAppNotificationsQuery>
{
    public GetAppNotificationsQueryValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Take)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(50);

        //RuleFor(x => x.OrderBy);

        //RuleFor(x => x.IsDescending);

       // RuleFor(x => x.Filter);

        //RuleFor(x => x.GroupBy);

        RuleFor(x => x.Search).MaximumLength(50);

        //RuleFor(x => x.SelectedColumns);
    }
}
