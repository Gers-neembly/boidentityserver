using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.Persistence.Contexts;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedClasses;
using System;
using System.Collections.Generic;
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

        public async Task<int> CreateBackOfficeUserById(string boUserId, int operatorId, BackOfficeUserInfo boUserInfo = null)
        {
            int resultBackOfficeId = 0;

            OperatorAssignment boOperator = CheckOperatorAssignment(boUserId, operatorId);

            if (boOperator != null)
                resultBackOfficeId = boOperator.BackOfficeId;
            else
            {
                long tagId = GlobalConstants.PlayerIdTagStarts;
                var operatorRecord = _appDBContext.OperatorData.Where(r => r.OperatorId == operatorId).FirstOrDefault();
                if (operatorRecord == null)
                    _appDBContext.OperatorData.Add(new OperatorData { OperatorId = operatorId, TagId = tagId });
                else
                {
                    tagId = operatorRecord.TagId + 1;
                    operatorRecord.TagId = tagId;
                    _appDBContext.Update(operatorRecord);
                }
                _appDBContext.OperatorAssignments.Add(new OperatorAssignment { NetUserId = boUserId, OperatorId = operatorId, BackOfficeId = (int) tagId});
                resultBackOfficeId = (int) tagId;
            };

            var boUserProfile = _appDBContext.BackOfficeUsers.Where(r => r.NetUserId.Equals(boUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (boUserProfile == null)
            {
                _appDBContext.BackOfficeUsers.Add(new BackOfficeUser
                {
                    NetUserId = boUserId,
                    FirstName = boUserInfo == null ? string.Empty : boUserInfo.FirstName,
                    LastName = boUserInfo == null ? string.Empty : boUserInfo.LastName,
                    MobilePrefix = boUserInfo == null ? string.Empty : boUserInfo.MobilePrefix,
                    MobileNo = boUserInfo == null ? string.Empty : boUserInfo.MobileNo,
                });
            }

            await _appDBContext.SaveChangesAsync();
            return (resultBackOfficeId);
        }

        public AppUser GetAppUser(string email, string username)
        {
            return _appDBContext.Users.Where(r => r.Email.ToLower() == email.ToLower()
                                             && r.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public async Task<bool> SetRegistrationStatus(string userId, RegistrationStatusNames registerStatus)
        {
            var boUser = await _appDBContext.Users.FindAsync(userId);
            if (boUser == null)
                return false;
            string strStatus = Enum.GetName(typeof(RegistrationStatusNames), registerStatus);
            if (boUser.RegistrationStatus.Equals(strStatus, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
            {
                boUser.RegistrationStatus = strStatus;
                return (await _appDBContext.SaveChangesAsync() > 0);
            }
        }

        public async Task<bool> ProfileRequestChange(string boUserId, BackOfficeUserInfo boUserInfo)
        {
            var boUser = _appDBContext.BackOfficeUsers.Where(r => r.NetUserId.Equals(boUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (boUser == null)
                return false;
            boUser.FirstName = boUserInfo.FirstName;
            boUser.LastName = boUserInfo.LastName;
            boUser.MobilePrefix = boUserInfo.MobilePrefix;
            boUser.MobileNo = boUserInfo.MobileNo;
            return (await _appDBContext.SaveChangesAsync() > 0);
        }

        public IEnumerable<int> GetOperatorAssignments(string netUserId)
        {
            return _appDBContext.OperatorAssignments.Where(r => r.NetUserId.Equals(netUserId, StringComparison.InvariantCultureIgnoreCase))
                                                          .Select(s => s.OperatorId).ToList();
        }

        public bool UserOperatorExists(string email, string username, int operatorId)
        {
            var appUser = _appDBContext.Users.Where(r => r.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                                                          || r.Email.ToLower() == email.ToLower()).FirstOrDefault();
            return (appUser == null) ? false : CheckOperatorAssignment(appUser.Id, operatorId) != null;
        }

        #region Private Methods
        private OperatorAssignment CheckOperatorAssignment(string netUserId, int operatorId)
        {
            return _appDBContext.OperatorAssignments.Where(r => r.NetUserId.Equals(netUserId, StringComparison.InvariantCultureIgnoreCase)
                                                    && r.OperatorId == operatorId).FirstOrDefault();
        }
        #endregion
    }
}
