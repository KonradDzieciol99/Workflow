using FluentValidation;

namespace Chat.Application.FriendRequests.Commands;

public class AcceptFriendRequestCommandValidator : AbstractValidator<AcceptFriendRequestCommand>
{
    public AcceptFriendRequestCommandValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty();
    }
}
