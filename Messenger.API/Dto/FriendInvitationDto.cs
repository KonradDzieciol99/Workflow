using Socjal.API.Entity;

namespace Socjal.API.Dto
{
    public class FriendInvitationDto
    {
        public string InviterUserEmail { get; set; }
        public string InvitedUserEmail { get; set; }
    }
}
