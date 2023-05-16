using MessageBus;

namespace IdentityServer.Common.Models
{
    public class RegisterEmailBusMessage:IntegrationEvent
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
