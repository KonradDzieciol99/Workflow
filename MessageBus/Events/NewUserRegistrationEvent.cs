using MediatR;

namespace MessageBus.Events
{
    public class NewUserRegistrationEvent : IntegrationEvent, IRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
