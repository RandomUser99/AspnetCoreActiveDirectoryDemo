using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ActiveDirectoryDemo.Infrastructure
{
    public class EmailClient : IEmailClient
    {
        private IConfiguration _configuration;

        public EmailClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string subject, string body, string from, string to)
        {
            var message = new MailMessage(from, to);
            message.Body = body;
            message.Subject = subject;

            var host = _configuration.GetValue<string>("Mail:SmtpHost");

            using (var client = new SmtpClient(host))
            {
                await client.SendMailAsync(message);
            }
        }

    }
}
