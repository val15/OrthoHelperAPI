using Microsoft.AspNetCore.Http;
using OrthoHelper.Domain.Features.Common.Ports;
using System.Security.Claims;

namespace OrthoHelper.Infrastructure.Features.Common.Services
{
    namespace OrthoHelper.Infrastructure.Features.Common.Services
    {
        public class CurrentUserService : ICurrentUserService
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            //public int UserId => int.Parse(_httpContextAccessor.HttpContext?.User?
            //    .FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            public int UserId => int.Parse(_httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value.ToString());

            //public string? Username => _httpContextAccessor.HttpContext.User
            //    .FindFirstValue(ClaimTypes.Name);

            public string? UserName => _httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.Name).Value.ToString();

            //   var userNameg = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);


            public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?
                .Identity?.IsAuthenticated ?? false;
        }
    }
}
