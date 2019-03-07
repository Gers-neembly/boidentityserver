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

        public async Task<bool> CreateBackOfficeUserById(string boUserId, int operatorId, BackOfficeUserInfo BackOfficeUserInfo = null)
        {
            // check and add backoffice user profile
            var boUserProfile = _appDBContext.BackOfficeUsers.Where(r => r.NetUserId.Equals(boUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (boUserProfile == null)
            {
                _appDBContext.BackOfficeUsers.Add(new BackOfficeUser
                {
                    NetUserId = boUserId,
                    FirstName = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.FirstName,
                    LastName = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.LastName,
                    MobilePrefix = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.MobilePrefix,
                    MobileNo = BackOfficeUserInfo == null ? string.Empty : BackOfficeUserInfo.MobileNo,
                });
            }

            // check and add backoffice user operatorId assignments
            OperatorAssignment boOperator = CheckOperatorAssignment(boUserId, operatorId);
            if (boOperator == null)
            {
                long tagId = 1;
                var operatorRecord = _appDBContext.OperatorData.Where(r => r.OperatorId == operatorId).FirstOrDefault();
                if (operatorRecord == null)
                    _appDBContext.OperatorData.Add(new OperatorData { OperatorId = operatorId, TagId = 1 });
                else
                {
                    tagId = operatorRecord.TagId + 1;
                    operatorRecord.TagId = tagId;
                }
                string tagFormatted = $"{operatorId}-{tagId:D8}";
                _appDBContext.OperatorAssignments.Add(new OperatorAssignment { NetUserId = boUserId, OperatorId = operatorId, BackOfficeUserId = tagFormatted });
            };
            return (await _appDBContext.SaveChangesAsync() > 0);
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

        public async Task<bool> ProfileRequestChange(string boUserId, BackOfficeUserInfo BackOfficeUserInfo)
        {
            var boUser = _appDBContext.BackOfficeUsers.Where(r => r.NetUserId.Equals(boUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (boUser == null)
                return false;
            boUser.FirstName = BackOfficeUserInfo.FirstName;
            boUser.LastName = BackOfficeUserInfo.LastName;
            boUser.MobilePrefix = BackOfficeUserInfo.MobilePrefix;
            boUser.MobileNo = BackOfficeUserInfo.MobileNo;
            return (await _appDBContext.SaveChangesAsync() > 0);
        }

        public IEnumerable<int> GetOperatorAssignments(string netUserId)
        {
            return _appDBContext.OperatorAssignments.Where(r => r.NetUserId.Equals(netUserId, StringComparison.InvariantCultureIgnoreCase))
                                                          .Select(s => s.OperatorId).ToList();
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
