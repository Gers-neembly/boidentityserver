using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedClasses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.Persistence.Helpers
{
    public class DataAccess : IDataAccess
    {
        private readonly AppDBContext _appDBContext;

        public DataAccess(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<string> CreateBackOfficeUserById(string userId, int operatorId, BackOfficeUserInfo BackOfficeUserInfo = null)
        {
            var BackOfficeUser = _appDBContext.Users.Where(r => r.Id.Equals(userId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (BackOfficeUser == null)
                return string.Empty;
            long tagId = 1;
            var operatorRecord = _appDBContext.OperatorData.Where(r => r.OperatorId == operatorId).FirstOrDefault();
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

        public AppUser GetAppUser(string email, string username, int operatorId)
        {
            var BackOfficeUserInfo =  _appDBContext.Users.Where(r => r.Email.ToLower() == email.ToLower()
                                                            && r.OperatorId == operatorId).FirstOrDefault();
            if (BackOfficeUserInfo != null)
              return BackOfficeUserInfo;

            BackOfficeUserInfo = _appDBContext.Users.Where(r => r.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                                                        && r.OperatorId == operatorId).FirstOrDefault();
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
