using System.Threading.Tasks;

namespace ActiveDirectoryDemo.Infrastructure
{
    public interface IEmailClient
    {
        Task SendAsync(string subject, string body, string from, string to);
    }
}