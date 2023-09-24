using FluentValidation;

namespace Notification.Application.AppNotifications.Queries;

public class GetAppNotificationsQueryValidator : AbstractValidator<GetAppNotificationsQuery>
{
    public GetAppNotificationsQueryValidator()
    {
        RuleFor(x => x.Skip).NotEmpty().GreaterThanOrEqualTo(0);

        RuleFor(x => x.Take).NotEmpty().GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);

        RuleFor(x => x.Search).MaximumLength(50);
    }
}
