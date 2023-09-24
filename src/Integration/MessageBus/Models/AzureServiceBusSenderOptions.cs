using System.ComponentModel.DataAnnotations;

namespace MessageBus.Models;

public class AzureServiceBusSenderOptions
{
    [Required]
    public required string ServiceBusConnectionString { get; set; }

    [Required]
    public required string TopicName { get; set; }
}
