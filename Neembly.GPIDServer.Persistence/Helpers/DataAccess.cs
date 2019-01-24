using Neembly.GPIDServer.Persistence.Entities;
using Neembly.GPIDServer.Persistence.Interfaces;
using Neembly.GPIDServer.SharedClasses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.GPIDServer.Persistence.Helpers
{
    public class DataAccess : IDataAccess
    {
        private readonly AppDBContext _appDBContext;

        public DataAccess(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<string> CreateBackOfficeUserById(string userId, string operatorId, BackOfficeUserInfo BackOfficeUserInfo = null)
        {
            var BackOfficeUser = _appDBContext.Users.Where(r => r.Id.Equals(userId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (BackOfficeUser == null)
                return string.Empty;
            long tagId = 1;
            var operatorRecord = _appDBContext.OperatorData.Where(r => r.OperatorId.Equals(operatorId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (operatorRecord == null)
            {
                _appDBContext.OperatorData.Add(new OperatorData { OperatorId = operatorId, TagId = 1 });
            }
            else
            {
                tagId = operatorRecord.TagId + 1;
                operatorRecord.TagId = tagId;
            }

            string tagFormatted = $"{operatorId}-{tagId:D8}";

            _appDBContext.BackOfficeUsers.Add(new BackOfficeUser
            { BackOfficeUserId = tagFormatted,
              FirstName = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.FirstName,
              LastName = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.LastName,
              MobilePrefix = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.MobilePrefix,
              MobileNo = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.MobileNo,
            });
            BackOfficeUser.BackOfficeUserId = tagFormatted;

            await _appDBContext.SaveChangesAsync();
            return tagFormatted;
        }

        public AppUser GetAppUser(string email, string username, string operatorId)
        {
            var BackOfficeUserInfo =  _appDBContext.Users.Where(r => r.Email.ToLower() == email.ToLower()
                                                            && r.OperatorId.Equals(operatorId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (BackOfficeUserInfo != null)
              return BackOfficeUserInfo;

            BackOfficeUserInfo = _appDBContext.Users.Where(r => r.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                                                        && r.OperatorId.Equals(operatorId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return BackOfficeUserInfo;
        }

        public async Task<bool> SetRegistrationStatus(string userId, RegistrationStatusNames registerStatus)
        {
            var BackOfficeUserInfo = await _appDBContext.Users.FindAsync(userId);
            if (BackOfficeUserInfo == null)
                return false;
            string strStatus = Enum.GetName(typeof(RegistrationStatusNames), registerStatus);
            if (BackOfficeUserInfo.RegistrationStatus.Equals(strStatus, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
            {
                BackOfficeUserInfo.RegistrationStatus = strStatus;
                return (await _appDBContext.SaveChangesAsync() > 0);
            }
        }

        public async Task<bool> ProfileRequestChange(string BackOfficeUserId, BackOfficeUserInfo BackOfficeUserInfo)
        {
            var BackOfficeUserRecord = _appDBContext.BackOfficeUsers.Where(r => r.BackOfficeUserId.Equals(BackOfficeUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (BackOfficeUserRecord == null)
                return false;
            BackOfficeUserRecord.FirstName = BackOfficeUserInfo.FirstName;
            BackOfficeUserRecord.LastName = BackOfficeUserInfo.LastName;
            BackOfficeUserRecord.MobilePrefix = BackOfficeUserInfo.MobilePrefix;
            BackOfficeUserRecord.MobileNo = BackOfficeUserInfo.MobileNo;
            return (await _appDBContext.SaveChangesAsync() > 0);
        }


    }
}
