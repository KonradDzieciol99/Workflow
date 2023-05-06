using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class ProjectMemberRemovedEvent : BaseMessage
    {
        public string ProjectMemberId { get; set; }
    }
}
