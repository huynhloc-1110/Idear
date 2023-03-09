using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Idear.Data.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<SendMailService> logger;

        public SendMailService (IOptions<MailSettings> mailSettings, ILogger<SendMailService> _logger)
        {
            _mailSettings = mailSettings.Value;
            logger = _logger;
            logger.LogInformation("Create SendMailService");
        }


        public async Task SendMail (MailContent mailContent)
        {

            var message = new MimeMessage();
            message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(mailContent.To));
            message.Subject = mailContent.Subject;


            //var url = Url.Action("Details", "Ideas", new { id = idea.Id }, Request.Scheme);

            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try {
                client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

                client.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                await client.SendAsync(message);

            } catch (Exception ex)
            {
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsaveFile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsaveFile);

                logger.LogInformation("There is an error while sending a mail, it is saved at " + emailsaveFile);
                logger.LogError(ex.Message);
            }

            client.Disconnect(true);
            
            logger.LogInformation("Send mail to " + mailContent.To);
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendMail(new MailContent()
            {
                To = email,
                Subject = subject,
                Body = htmlMessage
            });
        }

    }
}
