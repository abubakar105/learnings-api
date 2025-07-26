using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.CurrentLoggedInUser
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
    }
}
