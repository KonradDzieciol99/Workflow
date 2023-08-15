using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events
{
    public class ProjectMemberAddedEvent : IntegrationEvent
    {
        public ProjectMemberAddedEvent(string projectMemberId, string userId, string userEmail, string? photoUrl, int type, string projectId, int invitationStatus, string projectName, string projectIconUrl)
        {
            ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
            PhotoUrl = photoUrl;
            Type = type;
            ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
            InvitationStatus = invitationStatus;
            ProjectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
            this.projectIconUrl = projectIconUrl ?? throw new ArgumentNullException(nameof(projectIconUrl));
        }

        //public ProjectMemberAddedEvent(string projectMemberId, string userId, string userEmail, string? photoUrl, string projectId,int invitationStatus, int type = 2)
        //{
        //    ProjectMemberId = projectMemberId ?? throw new ArgumentNullException(nameof(projectMemberId));
        //    UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        //    UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        //    PhotoUrl = photoUrl;
        //    Type = type;
        //    ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
        //    InvitationStatus = invitationStatus;
        //}

        public string ProjectMemberId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string? PhotoUrl { get; set; }
        public int Type { get; set; }
        public string ProjectId { get; set; }
        public int InvitationStatus { get; set; }
        public string ProjectName { get; set; }
        public string projectIconUrl { get; set; }
    }
}
