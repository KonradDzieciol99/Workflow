using API.Aggregator.Application.Commons.Models;
using API.Aggregator.Infrastructure.Services;
using HttpMessage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Application.FriendRequestsAggregate.Queries;

public record SearchFriendAggregateQuery(string Email, int Take, int Skip)
    : IRequest<List<SearchedUserDto>>;

public class SearchFriendAggregateQueryHandler
    : IRequestHandler<SearchFriendAggregateQuery, List<SearchedUserDto>>
{
    private readonly IIdentityServerService _identityServerService;
    private readonly IChatService _chatService;

    public SearchFriendAggregateQueryHandler(
        IIdentityServerService identityServerService,
        IChatService chatService
    )
    {
        this._identityServerService =
            identityServerService ?? throw new ArgumentNullException(nameof(identityServerService));
        this._chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    }

    public async Task<List<SearchedUserDto>> Handle(
        SearchFriendAggregateQuery request,
        CancellationToken cancellationToken
    )
    {
        var usersFound = await _identityServerService.SearchAsync(
            request.Email,
            request.Take,
            request.Skip
        );

        if (usersFound is null || usersFound.Count == 0)
            return new List<SearchedUserDto>();

        var status = await _chatService.GetFriendsStatus(usersFound.Select(x => x.Id).ToList());

        return usersFound
            .Select(
                (x, index) => new SearchedUserDto(x.Id, x.Email, x.PhotoUrl, status[index].Status)
            )
            .ToList();
    }
}
