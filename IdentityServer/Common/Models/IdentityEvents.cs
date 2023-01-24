using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;

namespace IdentityServer.Common.Models
{
    public class IdentityEvents : IEventSink
    {
        public Task PersistAsync(Event evt)
        {
            Console.Write(evt.ToString());

            
            return Task.CompletedTask;
        }
    }
}
