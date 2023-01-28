
using Email.Common.Models;

namespace EmailSender
{
    public interface IEmailSender
    {
        public Task SendConfirmEmailMessage(RegisterEmailBusMessage registerEmailBusMessage);
    }
}
