using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.SharedClasses;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.Persistence.Interfaces
{
    public interface IDataAccess
    {
        AppUser GetAppUser(string email, string username, int operatorId);
        Task<string> CreateBackOfficeUserById(string userId, int operatorId, BackOfficeUserInfo BackOfficeUserInfo = null);
        Task<bool> SetRegistrationStatus(string userId, RegistrationStatusNames registerStatus);
        Task<bool> ProfileRequestChange(string BackOfficeUserId, BackOfficeUserInfo BackOfficeUserInfo);
    }
}
