using System.Threading.Tasks;

namespace Idear.Data.Services
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);
    }
}
