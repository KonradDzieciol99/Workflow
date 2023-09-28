namespace Chat.Application.Common.Models;

public class FriendRequestDtosWithTotalCount
{
    public int Count { get; set; }
    public required List<FriendRequestDto> Result { get; set; }
}
