using System;

namespace MessageBus.Models;

public class AzureServiceBusSubscriberOptions
{
    public string ServiceBusConnectionString { get; set; }
    public string SubscriptionName { get; set; }
    public void Validate()
    {
        if (string.IsNullOrEmpty(ServiceBusConnectionString))
        {
            throw new ArgumentException($"{nameof(ServiceBusConnectionString)} cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(SubscriptionName))
        {
            throw new ArgumentException($"{nameof(SubscriptionName)} cannot be null or empty.");
        }
    }
}
