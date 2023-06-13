namespace SignalR.Models;

public class ChatGroupMember
{
    public string UserEmail { get; set; }
    public bool IsTyping { get; set; } = false;
}
