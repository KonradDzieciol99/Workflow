using Email.Common.Models;

namespace Email
{
    public interface IEmailSender
    {
        public Task SendConfirmEmailMessage(RegisterEmailBusMessage registerEmailBusMessage);
    }
}
