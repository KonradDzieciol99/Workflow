using System.Threading.Tasks;

namespace MessageBus;

public interface IAzureServiceBusSender
{
    Task PublishMessage(IntegrationEvent message);
}
