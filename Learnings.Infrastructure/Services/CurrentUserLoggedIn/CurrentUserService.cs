using Learnings.Application.Services.CurrentLoggedInUser;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.CurrentUserLoggedIn
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _ctx;
        public CurrentUserService(IHttpContextAccessor ctx) => _ctx = ctx;
        public string UserId =>
            _ctx.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
