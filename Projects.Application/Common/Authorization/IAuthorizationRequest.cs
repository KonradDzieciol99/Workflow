using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Common.Authorization
{
    public interface IAuthorizationRequest
    {
        public List<IAuthorizationRequirement> GetAuthorizationRequirement();
    }
}
