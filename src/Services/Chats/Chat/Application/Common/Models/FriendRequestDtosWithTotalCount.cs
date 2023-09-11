namespace Chat.Application.Common.Models;

public class FriendRequestDtosWithTotalCount
{
    public int Count { get; set; }
    public List<FriendRequestDto> Result { get; set; }
}
