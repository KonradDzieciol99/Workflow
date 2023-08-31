using FluentValidation;

namespace Chat.Application.FriendRequests.Commands;

public class CreateFriendRequestCommandValidator: AbstractValidator<CreateFriendRequestCommand>
{
    public CreateFriendRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        //RuleFor(x => x.PhotoUrl);
    }
}