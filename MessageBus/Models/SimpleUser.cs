using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models
{
    public class SimpleUser
    {
        public SimpleUser()
        {
            
        }
        public SimpleUser(string userId, string userEmail, string? userPhotoUrl)
        {
            UserId = userId;
            UserEmail = userEmail;
            this.userPhotoUrl = userPhotoUrl;
        }

        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string? userPhotoUrl { get; set; }
    }
}
