using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models;

public class RabbitMQSenderOptions
{
    [Required]
    public required string RabbitMQConnectionString { get; set; }

    [Required]
    public required string Exchange { get; set; }

    [Required]
    public required string Queue { get; set; }
}
