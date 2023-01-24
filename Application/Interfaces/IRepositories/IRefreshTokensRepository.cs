using Core.Interfaces.IRepositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IRefreshTokensRepository : IRepository<RefreshToken>
    {
    }
}
