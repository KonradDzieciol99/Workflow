using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using WorkflowApi.Exceptions;

namespace WebAPI.Services
{
    public class CookiesService : ICookiesService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;

        public CookiesService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContext = _httpContextAccessor.HttpContext ?? throw new ArgumentNullException("Empty httpContext");
        }

        public Guid GetRefreshToken()
        {
            string? refreshTokenAsString = _httpContext.Request.Cookies["Workflow-Refresh-Token"];

            if (string.IsNullOrEmpty(refreshTokenAsString))
            {
                throw new BadRequestException("Empty cookie");
            }

            if (Guid.TryParse(refreshTokenAsString, out Guid refreshTokenAsGuid))
            {
                return refreshTokenAsGuid;
            }

            throw new BadRequestException("Bad format of cookie");
        }

        public void RemoveRefreshToken()
        {
            _httpContext.Response.Cookies.Delete("Workflow-Refresh-Token");
        }

        public void SetRefreshToken(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1),
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true
            };
            _httpContext.Response.Cookies.Append("Workflow-Refresh-Token", refreshToken, cookieOptions);


        }
    }
}
