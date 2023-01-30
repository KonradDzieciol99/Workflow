﻿namespace Socjal.API.Entity
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? PhotoUrl { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }

    }

}
