using Domain.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public Guid Token{ get; set; }
        public bool IsUsed { get; set; } = true;
        public bool IsRevoked { get; set; } = false;
        public DateTime AddedDate { get; set; } = new DateTime();
        public DateTime ExpiryDate { get; set; } = new DateTime().AddDays(7);
        public int UserId { get; set; }
        public AppUser AppUser { get; set; }

        //
    }
}
