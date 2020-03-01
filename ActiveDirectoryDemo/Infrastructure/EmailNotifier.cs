using System.Threading.Tasks;

namespace ActiveDirectoryDemo.Infrastructure
{
    public class EmailNotifier : INotifier
    {
        private IEmailClient _client;

        public EmailNotifier(IEmailClient client)
        {
            _client = client;
        }

        public async Task SendAsync(string subject, string body, string from, string to)
        {
            await _client.SendAsync(subject, body, from, to);
        }
    }
}
