using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedClasses.Outputs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.Persistence.Interfaces
{
    public interface IDataAccess
    {
        AppUser GetAppUser(string email, string username);
        AppUser GetAppUserById(string Id);
        bool UserOperatorExists(string email, string username);
        Task<int> CreateBackOfficeUserById(string boUserId, int operatorId, BackOfficeUserInfo boUserInfo = null);
        Task<bool> SetRegistrationStatus(string userId, BOUserStatus registerStatus);
        Task<bool> ProfileRequestChange(string BackOfficeUserId, BackOfficeUserInfo boUserInfo);
        IEnumerable<int> GetOperatorAssignments(string netUserId);
        UserInfo GetUserInfo(string username);
        Task<UserInfoViewModel> GetUsers(int operatorId, int pageIndex, int pageSize);
        Task<List<ClaimsViewModel>> GetUserClaims(string userId);
        Task<bool> SetPasswordResetStatus(string userId, bool status);
        bool UpdateEmailExist(string email, string username);
    }
}
