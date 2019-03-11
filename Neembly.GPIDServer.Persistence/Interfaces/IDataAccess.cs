using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.SharedClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.Persistence.Interfaces
{
    public interface IDataAccess
    {
        AppUser GetAppUser(string email, string username);
        bool UserOperatorExists(string email, string username, int operatorId);
        Task<int> CreateBackOfficeUserById(string boUserId, int operatorId, BackOfficeUserInfo boUserInfo = null);
        Task<bool> SetRegistrationStatus(string userId, RegistrationStatusNames registerStatus);
        Task<bool> ProfileRequestChange(string BackOfficeUserId, BackOfficeUserInfo boUserInfo);
        IEnumerable<int> GetOperatorAssignments(string netUserId);
    }
}
