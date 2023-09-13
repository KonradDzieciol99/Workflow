using System.Threading.Tasks;

namespace MessageBus;
public interface IEventBusConsumer
{
    Task Subscribe<T>() where T : IntegrationEvent;
}