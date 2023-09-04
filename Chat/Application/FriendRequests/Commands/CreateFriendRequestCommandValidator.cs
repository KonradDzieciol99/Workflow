using FluentValidation;

namespace Chat.Application.FriendRequests.Commands;

public class CreateFriendRequestCommandValidator: AbstractValidator<CreateFriendRequestCommand>
{
    public CreateFriendRequestCommandValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty();
        RuleFor(x => x.TargetUserEmail).NotEmpty().EmailAddress();
        //RuleFor(x => x.PhotoUrl);
    }
}