using Learnings.Application.Dtos;
using Learnings.Infrastructure.Mail.InterfaceService;
using Microsoft.Extensions.Options;
using MimeKit;

using MailKit.Net.Smtp;

public class MailService : IMailService
{
    MailSettings Mail_Settings = null;
    public MailService(IOptions<MailSettings> options)
    {
        Mail_Settings = options.Value;
    }
    public bool SendMail(MailData Mail_Data)
    {
        try
        {
            MimeMessage email_Message = new MimeMessage();
            email_Message.From.Add(new MailboxAddress(Mail_Settings.Name, Mail_Settings.EmailId));
            email_Message.To.Add(new MailboxAddress(Mail_Data.EmailToName, Mail_Data.EmailToId));
            email_Message.Subject = Mail_Data.EmailSubject;

            var emailBodyBuilder = new BodyBuilder { HtmlBody = Mail_Data.EmailBody };
            email_Message.Body = emailBodyBuilder.ToMessageBody();

            using (var MailClient = new SmtpClient())
            {
                MailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                MailClient.Authenticate(Mail_Settings.EmailId, Mail_Settings.Password);

                MailClient.Send(email_Message);
                MailClient.Disconnect(true);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

}