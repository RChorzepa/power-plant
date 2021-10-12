using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Services.MailService
{
    public interface IMailService
    {
        Task SendMessage();
    }
}
