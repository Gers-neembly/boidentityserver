using Neembly.BOIDServer.SharedClasses;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.SharedServices.Interfaces
{
    public interface IEmailDispatcher
    {
        Task SendActivationLink(string emailLink, string name, string toEmail);
        Task SendWelcomeEmail(string referer, string name, string toEmail);
    }
}
