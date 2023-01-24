using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class RevokeRefreshTokenCommand:IRequest<Unit>
    {
        public RevokeRefreshTokenCommand()
        {

        }
    }
}
