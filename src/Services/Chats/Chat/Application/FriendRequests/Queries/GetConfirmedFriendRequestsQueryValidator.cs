using FluentValidation;

namespace Chat.Application.FriendRequests.Queries;

public class GetConfirmedFriendRequestsQueryValidator
    : AbstractValidator<GetConfirmedFriendRequestsQuery>
{
    public GetConfirmedFriendRequestsQueryValidator()
    {
        RuleFor(x => x.Skip).NotNull();

        RuleFor(x => x.Take).NotNull().GreaterThan(0).LessThanOrEqualTo(50);

        RuleFor(x => x.Search).MaximumLength(50);
    }
}
