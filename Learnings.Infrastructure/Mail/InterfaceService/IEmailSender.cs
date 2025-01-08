using Learnings.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Mail.InterfaceService
{
    public interface IMailService
    {
        bool SendMail(MailData Mail_Data);
    }
}
