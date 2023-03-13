namespace IdentityServer.Common.Models
{
    public class SearchedUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? PhotoUrl { get; set; }
        //public bool IsAlreadyInvited { get; set; }
        //public bool Confirmed { get; set; }
        public UserFriendStatusType Status { get; set; } = UserFriendStatusType.Stranger;

    }
}
