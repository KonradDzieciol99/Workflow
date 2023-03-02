using Mango.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus
{
    public interface IAzureServiceBusSender
    {
        Task PublishMessage<T>(T message, string queueOrTopicName) where T : BaseMessage;
    }
}
