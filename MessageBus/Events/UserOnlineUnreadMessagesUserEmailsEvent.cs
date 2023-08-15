using MessageBus.Models;
using System;
using System.Collections.Generic;

namespace MessageBus.Events;

public class UserOnlineUnreadMessagesUserEmailsEvent : IntegrationEvent
{
    public UserOnlineUnreadMessagesUserEmailsEvent(UserDto onlineUser, List<string> emails)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
        Emails = emails ?? throw new ArgumentNullException(nameof(emails));
    }

    public UserDto OnlineUser { get; set; }
    public List<string> Emails { get; set; }
}
