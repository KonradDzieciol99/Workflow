﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models
{
    public class SimpleUser
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string? userPhotoUrl { get; set; }
    }
}