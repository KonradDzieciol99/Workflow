using System.ComponentModel.DataAnnotations;

namespace MessageBus.Models;

public class AzureServiceBusConsumerOptions
{
    [Required]
    public required string ServiceBusConnectionString { get; set; }
    [Required]
    public required string SubscriptionName { get; set; }
    [Required]
    public required string TopicName { get; set; }
}
