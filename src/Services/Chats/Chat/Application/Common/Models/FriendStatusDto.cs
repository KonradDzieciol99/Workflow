namespace Chat.Application.Common.Models;

public class FriendStatusDto
{
    public FriendStatusDto() { }

    public FriendStatusDto(string userId, FriendStatusType status)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Status = status;
    }

    public string UserId { get; set; }
    public FriendStatusType Status { get; set; }
}
