using MediatR;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents.Handlers;

public class UserConnectedToChatResponseEventHandler : IRequestHandler<UserConnectedToChatResponseEvent>
{
    private readonly IHubContext<ChatHub> _chatHubContext;

    public UserConnectedToChatResponseEventHandler(IHubContext<ChatHub> chatHubContext)
    {
        this._chatHubContext = chatHubContext;
    }
    public async Task Handle(UserConnectedToChatResponseEvent request, CancellationToken cancellationToken)
    {
        await _chatHubContext.Clients.User(request.ConnectedUser.Id).SendAsync("ReceiveMessageThread", request.Messages);
        return;
    }
}
