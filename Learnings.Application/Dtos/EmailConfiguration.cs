using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos
{
    public class MailSettings
    {
        public string EmailId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
    }
    public class MailData
    {
        public string EmailToId { get; set; }
        public string? EmailToName { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }
    }
    public class OTPDetails
    {
        public string Code { get; set; }  // OTP Code
        public DateTime ExpiryTime { get; set; }  // OTP Expiry Time
        public string Token { get; set; }  // OTP Expiry Time
    }
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

}
