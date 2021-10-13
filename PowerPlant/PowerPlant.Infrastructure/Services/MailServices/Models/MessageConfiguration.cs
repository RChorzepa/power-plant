using PowerPlant.Infrastructure.Services.MailServices.Models;
using System.Collections.Generic;

namespace PowerPlant.Infrastructure.Services.MailService.Models
{
    public class MessageConfiguration
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}
