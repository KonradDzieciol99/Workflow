using FluentValidation;

namespace Chat.Application.FriendRequests.Queries;

public class GetReceivedFriendRequestsQueryValidator
    : AbstractValidator<GetReceivedFriendRequestsQuery>
{
    public GetReceivedFriendRequestsQueryValidator()
    {
        RuleFor(x => x.Skip).NotNull();

        RuleFor(x => x.Take).NotNull().GreaterThan(0).LessThanOrEqualTo(50);

        RuleFor(x => x.Search).MaximumLength(50);
    }
}
