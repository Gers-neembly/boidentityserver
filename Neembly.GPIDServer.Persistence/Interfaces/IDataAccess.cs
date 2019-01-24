using Neembly.GPIDServer.Persistence.Entities;
using Neembly.GPIDServer.SharedClasses;
using System.Threading.Tasks;

namespace Neembly.GPIDServer.Persistence.Interfaces
{
    public interface IDataAccess
    {
        AppUser GetAppUser(string email, string username, string operatorId);
        Task<string> CreateBackOfficeUserById(string userId, string operatorId, BackOfficeUserInfo BackOfficeUserInfo = null);
        Task<bool> SetRegistrationStatus(string userId, RegistrationStatusNames registerStatus);
        Task<bool> ProfileRequestChange(string BackOfficeUserId, BackOfficeUserInfo BackOfficeUserInfo);
    }
}
