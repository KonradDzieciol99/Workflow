using Mango.MessageBus;

namespace MessageBus.Events
{
    public class NewUserRegisterEmail : BaseMessage
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
