using Mango.MessageBus;

namespace Email.Common.Models
{
    public class NewUserRegisterEmail : BaseMessage
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
