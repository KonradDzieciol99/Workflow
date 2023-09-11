using FluentValidation;

namespace Notification.Application.AppNotifications.Commands;

public class DeleteAppNotificationCommandValidator : AbstractValidator<DeleteAppNotificationCommand>
{
    public DeleteAppNotificationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
