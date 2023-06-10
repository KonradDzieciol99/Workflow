using FluentValidation;

namespace Notification.Application.AppNotifications.Commands;

public class MarkAsSeenAppNotificationCommandValidator : AbstractValidator<MarkAsSeenAppNotificationCommand>
{
    public MarkAsSeenAppNotificationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
