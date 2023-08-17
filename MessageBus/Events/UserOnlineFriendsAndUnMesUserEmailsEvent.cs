﻿using MediatR;
using MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Events;

public class UserOnlineFriendsAndUnMesUserEmailsEvent : IntegrationEvent
{
    public UserOnlineFriendsAndUnMesUserEmailsEvent(UserDto onlineUser, List<UserDto> listOfAcceptedFriends, List<string> unreadMessagesUserEmails)
    {
        OnlineUser = onlineUser ?? throw new ArgumentNullException(nameof(onlineUser));
        ListOfAcceptedFriends = listOfAcceptedFriends ?? throw new ArgumentNullException(nameof(listOfAcceptedFriends));
        UnreadMessagesUserEmails = unreadMessagesUserEmails ?? throw new ArgumentNullException(nameof(unreadMessagesUserEmails));
    }

    public UserDto OnlineUser { get; set; }
    public List<UserDto> ListOfAcceptedFriends { get; set; }
    public List<string> UnreadMessagesUserEmails { get; set; }
}