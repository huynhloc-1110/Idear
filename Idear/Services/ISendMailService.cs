using System.Threading.Tasks;

namespace Idear.Services
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);
    }
}
