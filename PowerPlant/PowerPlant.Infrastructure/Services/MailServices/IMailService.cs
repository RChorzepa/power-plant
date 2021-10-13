using System.Threading.Tasks;
using PowerPlant.Infrastructure.Services.MailService.Models;

namespace PowerPlant.Infrastructure.Services.MailService
{
    public interface IMailService
    {
        Task SendMessage(MessageConfiguration messageConfiguration);
    }
}
