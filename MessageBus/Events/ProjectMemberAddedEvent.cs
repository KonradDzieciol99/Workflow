using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class ProjectMemberAddedEvent : BaseMessage
    {
        public string ProjectMemberId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string PhotoUrl { get; set; }
        public ProjectMemberType Type { get; set; }
        public string ProjectId { get; set; }
    }
}
