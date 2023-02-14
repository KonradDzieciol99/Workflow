﻿namespace Chat.Dto
{
    public class UserSearchedFriendInvitationDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsAlreadyInvited { get; set; }
        public bool Confirmed { get; set; }
    }
}