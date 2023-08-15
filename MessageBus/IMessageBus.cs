using System;
using System.Threading.Tasks;

namespace MessageBus;

public interface IMessageBus
{
    Task PublishMessage<T>(T message, string queueOrTopicName);
}
