using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace MessageBus;
public interface IEventBusConsumer : IHostedService , IDisposable
{
    Task Subscribe<T>() where T : IntegrationEvent;
}