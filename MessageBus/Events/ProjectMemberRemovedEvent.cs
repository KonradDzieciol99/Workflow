using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class ProjectMemberRemovedEvent : BaseMessage
    {
        public string UserId { get; set; }
        public string ProjectId { get; set; }

    }
}
