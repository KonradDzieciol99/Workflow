
using Email.Common.Models;

namespace EmailSender
{
    public interface IEmailSender
    {
        public Task SendConfirmEmailMessage(NewUserRegisterEmail registerEmailBusMessage);
    }
}
