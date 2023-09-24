using FluentValidation;

namespace Chat.Application.FriendRequests.Commands;

public class DeleteFriendRequestCommandValidator : AbstractValidator<DeleteFriendRequestCommand>
{
    public DeleteFriendRequestCommandValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty();
    }
}
