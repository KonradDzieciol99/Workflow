using Mango.MessageBus;

namespace IdentityServer.Common.Models
{
    public class RegisterEmailBusMessage:BaseMessage
    {
        public string Email { get; set; }
    }
}
