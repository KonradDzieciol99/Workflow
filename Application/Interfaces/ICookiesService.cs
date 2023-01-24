using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICookiesService
    {
        Guid GetRefreshToken();
        void SetRefreshToken(string refreshToken);
        void RemoveRefreshToken();
    }
}
