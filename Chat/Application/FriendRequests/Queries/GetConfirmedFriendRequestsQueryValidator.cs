using FluentValidation;

namespace Chat.Application.FriendRequests.Queries;

public class GetConfirmedFriendRequestsQueryValidator: AbstractValidator<GetConfirmedFriendRequestsQuery>
{
    public GetConfirmedFriendRequestsQueryValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(50);

        RuleFor(x => x.Search).MaximumLength(50);
    }
}