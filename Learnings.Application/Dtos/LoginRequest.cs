using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos
{
    public class LoginDto
    {
        //public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

    }
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }
    public class CheckDuplicateUser
    {
        public string Email { get; set; }
    }

}
