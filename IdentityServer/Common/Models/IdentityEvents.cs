using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Mango.MessageBus;

namespace IdentityServer.Common.Models
{
    public class IdentityEvents : IEventSink
    {
        private readonly IMessageBus _messageBus;

        public IdentityEvents(IMessageBus messageBus)
        {
            this._messageBus = messageBus;
        }
        public async Task PersistAsync(Event evt)
        {
            if (evt.Name == "Local User Register")
            {
                var localUserRegisterSuccessEvent = (LocalUserRegisterSuccessEvent)evt;
                var registerEmailBusMessage =  new RegisterEmailBusMessage() { Email = localUserRegisterSuccessEvent.LocalUserEmail };
                await _messageBus.PublishMessage(registerEmailBusMessage, "newuserregister");
            }
            

            Console.Write(evt.ToString());
            
            return;
        }
    }
}
