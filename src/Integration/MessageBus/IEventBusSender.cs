using System.Threading.Tasks;

namespace MessageBus;

public interface IEventBusSender
{
    Task PublishMessage(IntegrationEvent message);
}
