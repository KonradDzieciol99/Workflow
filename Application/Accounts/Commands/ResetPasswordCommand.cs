using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class ResetPasswordCommand:IRequest<Unit>
    {
        public ResetPasswordCommand(string email)
        {
            Email = email;
        }
        public string Email { get; set; }
    }
}
