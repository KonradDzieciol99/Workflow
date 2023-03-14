namespace Chat.Entity
{
    public class FriendInvitation
    {
        //public User InviterUser { get; set; }
        public string InviterUserId { get; set; }
        public string InviterUserEmail { get; set; }
        public string? InviterPhotoUrl { get; set; }
        //public User InvitedUser { get; set; }
        public string InvitedUserId { get; set; }
        public string InvitedUserEmail { get; set; }
        public string? InvitedPhotoUrl { get; set; }
        public bool Confirmed { get; set; } = false;
    }
}
