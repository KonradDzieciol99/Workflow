using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RefreshTokensRepository : Repository<RefreshToken>, IRefreshTokensRepository
    {
        public RefreshTokensRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
