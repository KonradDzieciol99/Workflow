using Mango.MessageBus;
using MediatR;

namespace MessageBus.Events
{
    public class NewUserRegistrationEvent : BaseMessage, IRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
