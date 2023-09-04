using MediatR;
using MessageBus;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons.Models;
using SignalR.Hubs;

namespace SignalR.IntegrationEvents;

public class UserConnectedToChatResponseEvent : IntegrationEvent
{
    public UserConnectedToChatResponseEvent(UserDto connectedUser, string recipientEmail, List<MessageDto> messages)
    {
        ConnectedUser = connectedUser ?? throw new ArgumentNullException(nameof(connectedUser));
        RecipientEmail = recipientEmail ?? throw new ArgumentNullException(nameof(recipientEmail));
        Messages = messages ?? throw new ArgumentNullException(nameof(messages));
    }

    public UserDto ConnectedUser { get; set; }
    public string RecipientEmail { get; set; }
    public List<MessageDto> Messages { get; set; }
}
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
