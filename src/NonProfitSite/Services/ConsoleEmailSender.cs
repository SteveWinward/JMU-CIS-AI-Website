using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NonProfitSite.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;
        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            _logger.LogInformation("--- Sending email (console) ---\nTo: {To}\nSubject: {Subject}\nBody: {Body}", to, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
