using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PowerPlant.Infrastructure.Services.MailService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Services.MailService
{
    public class SmtpMailService : IMailService
    {
        private readonly SmtpConfiguration _smtp;

        public SmtpMailService(IOptions<SmtpConfiguration> configuration)
        {
            _smtp = configuration.Value;
        }

        public async Task SendMessage(MessageConfiguration message)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(message.From, message.From);

            for (int i = 0; i < message.To.Length; i++)
                mailMessage.To.Add(message.To[i]);

            mailMessage.Subject = message.Subject;
            mailMessage.Body = message.Body;
            mailMessage.IsBodyHtml = true;

            if (message.Attachments.Any())
                foreach (var attchemnt in message.Attachments)
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(attchemnt.Content), attchemnt.Name));


            var smtp = new SmtpClient(_smtp.Host)
            {
                Port = _smtp.Port,
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);
        }
    }
}
