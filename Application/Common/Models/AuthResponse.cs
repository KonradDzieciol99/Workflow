using Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public bool IsEmailVerified { get; set; }
        public UserDto? UserDto { get; set; }
        public string? RedirectLink { get; set; }
        public string? Email { get; set; }
    }
}
