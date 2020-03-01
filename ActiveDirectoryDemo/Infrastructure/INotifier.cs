using System.Threading.Tasks;

namespace ActiveDirectoryDemo.Infrastructure
{
    public interface INotifier
    {
        Task SendAsync(string subject, string body, string from, string to);
    }
}