using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models
{
    public class AzureServiceBusSubscriberOptions
    {
        public Dictionary<string, Type> QueueNameAndEventTypePair { get; set; }
        //public Dictionary<Tuple<string, string>, Type> TopicNameWithSubscriptionNameAndEventTypePair { get; set; }
        public Dictionary<string, Type> TopicNameAndEventTypePair { get; set; }
        public Dictionary<string, string> TopicNameWithSubscriptionName { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
