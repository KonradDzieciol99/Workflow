using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models;

public class RabbitMQConsumerOptions
{
    [Required]
    public required string Host { get; set; }
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Exchange { get; set; }
    [Required]
    public required string Queue { get; set; }
}
