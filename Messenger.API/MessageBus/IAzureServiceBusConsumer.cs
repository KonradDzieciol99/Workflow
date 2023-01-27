using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Socjal.API.MessageBus
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
