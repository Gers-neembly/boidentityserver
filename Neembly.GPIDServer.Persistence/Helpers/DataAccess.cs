using Microsoft.EntityFrameworkCore;
using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.Persistence.Contexts;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedClasses.Outputs;
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
                _appDBContext.OperatorAssignments.Add(new OperatorAssignment { NetUserId = boUserId, OperatorId = operatorId, BackOfficeId = (int)tagId });
                resultBackOfficeId = (int)tagId;
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
                    InitialPassword = boUserInfo == null ? string.Empty : boUserInfo.InitialPassword,
                    IsPasswordReset = boUserInfo == null ? false : boUserInfo.IsPasswordReset,
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

        public AppUser GetAppUserById(string Id)
        {
            return _appDBContext.Users.Where(r => r.Id == Id).FirstOrDefault();
        }


        public async Task<bool> SetRegistrationStatus(string userId, BOUserStatus registerStatus)
        {
            var boUser = await _appDBContext.Users.FindAsync(userId);
            if (boUser == null)
                return false;
            string strStatus = Enum.GetName(typeof(BOUserStatus), registerStatus);
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

        public bool UserOperatorExists(string email, string username)
        {
            var appUser = _appDBContext.Users.Where(r => r.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                                                          || r.Email.ToLower() == email.ToLower()).FirstOrDefault();
            return (appUser != null);
        }

        public bool UpdateEmailExist(string email, string username)
        {
            var appUser = _appDBContext.Users.Where(r => r.Email.ToLower() == email.ToLower()).FirstOrDefault();
            return (appUser != null && appUser.UserName != username);
        }

        #region Private Methods
        private OperatorAssignment CheckOperatorAssignment(string netUserId, int operatorId)
        {
            return _appDBContext.OperatorAssignments.Where(r => r.NetUserId.Equals(netUserId, StringComparison.InvariantCultureIgnoreCase)
                                                    && r.OperatorId == operatorId).FirstOrDefault();
        }
        #endregion

        #region UAC Methods

        public UserInfo GetUserInfo(string username)
        {
            var userInfo = _appDBContext.Users.Join(_appDBContext.BackOfficeUsers,
                user => user.Id,
                boinfo => boinfo.NetUserId,
                (user, boinfo) => new UserInfo
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    FirstName = boinfo.FirstName,
                    LastName = boinfo.LastName,
                    Email = user.Email,
                    Status = user.RegistrationStatus == RegistrationStatusNames.Registered.ToString() || user.RegistrationStatus == BOUserStatus.Active.ToString() ? BOUserStatus.Active.ToString() : BOUserStatus.Inactive.ToString(),
                    CreatedDate = user.CreatedDate,
                    ModifiedDate = user.ModifiedDate,
                    IsPasswordReset = boinfo.IsPasswordReset
                })
                .Where(r => r.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            return userInfo;
        }

        public List<UserInfo> GetUsers(int operatorId)
        {
            var userlist = _appDBContext.Users
                .Join(_appDBContext.OperatorAssignments.Where(r => r.OperatorId == operatorId)
                    , users => users.Id, op => op.NetUserId,
                    (users, op) => new { userTable = users, operatorInfo = op })
                .Join(_appDBContext.BackOfficeUsers,
                user => user.userTable.Id,
                boinfo => boinfo.NetUserId,
                (user, boinfo) => new UserInfo
                {
                    UserId = user.userTable.Id,
                    Username = user.userTable.UserName,
                    FirstName = boinfo.FirstName,
                    LastName = boinfo.LastName,
                    Email = user.userTable.Email,
                    Status = user.userTable.RegistrationStatus == RegistrationStatusNames.Registered.ToString() || user.userTable.RegistrationStatus == BOUserStatus.Active.ToString() ? BOUserStatus.Active.ToString() : BOUserStatus.Inactive.ToString(),
                    OperatorId = user.operatorInfo.OperatorId,
                    CreatedDate = user.userTable.CreatedDate,
                    ModifiedDate = user.userTable.ModifiedDate
                }).ToList();

            return userlist;
        }

        public async Task<List<ClaimsViewModel>> GetUserClaims(string userId)
        {
            var claims = await _appDBContext.UserClaims
                .Where(r => r.UserId == userId)
                .Select(s => new ClaimsViewModel
                {
                    UserId = s.UserId,
                    ClaimType = s.ClaimType,
                    ClaimValue = s.ClaimValue
                }).ToListAsync();

            return claims;
        }


        public async Task<bool> SetPasswordResetStatus(string userId, bool status)
        {
            var boUser = _appDBContext.BackOfficeUsers.Where(r => r.NetUserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (boUser == null)
                return false;
            boUser.IsPasswordReset = status;
            return (await _appDBContext.SaveChangesAsync() > 0);
        }
        #endregion
    }
}
