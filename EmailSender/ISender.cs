using MessageBus.Events;

namespace EmailSender
{
    public interface ISender
    {
        Task CreateConfirmEmailMessage(NewUserRegistrationEvent registerEmailBusMessage);
    }
}