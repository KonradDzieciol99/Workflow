using MessageBus.Events;

namespace EmailSender
{
    public interface IEmailSender
    {
        public Task SendConfirmEmailMessage(NewUserRegistrationEvent registerEmailBusMessage);
    }
}
