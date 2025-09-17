using System.Threading.Tasks;

namespace NonProfitSite.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
}
