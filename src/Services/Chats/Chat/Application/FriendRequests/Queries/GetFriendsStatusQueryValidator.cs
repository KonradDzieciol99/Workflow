using FluentValidation;

namespace Chat.Application.FriendRequests.Queries;

public class GetFriendsStatusQueryValidator : AbstractValidator<GetFriendsStatusQuery>
{
    public GetFriendsStatusQueryValidator()
    {
        RuleFor(x => x.UsersIds).NotNull().Must(list => list.Count > 0);
    }
}
