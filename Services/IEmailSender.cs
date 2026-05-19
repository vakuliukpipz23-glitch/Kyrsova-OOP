using System.Threading;
using System.Threading.Tasks;

namespace Kyrsova_OOP.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string subject, string body, CancellationToken cancellationToken);
    }
}
