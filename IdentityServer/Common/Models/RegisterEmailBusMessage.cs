using MessageBus;

namespace IdentityServer.Common.Models
{
    public class RegisterEmailBusMessage:BaseMessage
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
