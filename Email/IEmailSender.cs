using MessageBus.Events;

namespace Email
{
    public interface IEmailSender
    {
        public Task SendConfirmEmailMessage(NewUserRegisterEmail registerEmailBusMessage);
    }
}
