using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Neembly.BOIDServer.Persistence.Contexts;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.Persistence.Seeders.Models;
using Neembly.BOIDServer.SharedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.Persistence.Seeders
{
    public static class BOUsers
    {
        #region constants
        private const int operatorId0 = 200;
        private const int operatorId1 = 300;
        private const int operatorId2 = 350;

        private const string UserDev1 = "rea";
        private const string UserDev2 = "mel";

        private const string UserQa1 = "ralph";
        private const string UserQa2 = "rob";
        private const string UserQa3 = "qa";

        private const string UserTag1 = "bouser";
        private const string UserTag2 = "devuser";

        private const string UserRole1 = "testerA";
        private const string UserRole2 = "testerB";

        private const string UserEmailTag = "@gmail.com";
        private const string UserDefaultPassword = "password";

        #endregion

        #region Local Data Definitions
        private static readonly List<RegisterUser> localUserAccount = new List<RegisterUser>
        {
            { new RegisterUser{Email = UserTag1+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserTag1+operatorId1, OperatorId = operatorId0, Role = UserRole1} },
            { new RegisterUser{Email = UserTag1+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserTag1+operatorId1, OperatorId = operatorId1, Role = UserRole1} },
            { new RegisterUser{Email = UserTag1+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserTag1+operatorId2, OperatorId = operatorId2, Role = UserRole1} },
            { new RegisterUser{Email = UserTag2+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserTag2+operatorId0, OperatorId = operatorId0, Role = UserRole2} },
            { new RegisterUser{Email = UserTag2+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserTag2+operatorId1, OperatorId = operatorId1, Role = UserRole2} },
            { new RegisterUser{Email = UserTag2+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserTag2+operatorId2, OperatorId = operatorId2, Role = UserRole2} },
        };
        #endregion

        #region Dev Data Definitions
        private static readonly List<RegisterUser> devUserAccount = new List<RegisterUser>
        {
            { new RegisterUser{Email = UserDev1+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserDev1+operatorId1, OperatorId = operatorId1, Role = UserRole1} },
            { new RegisterUser{Email = UserDev2+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserDev2+operatorId2, OperatorId = operatorId2, Role = UserRole2} },
        };
        #endregion

        #region QA Data Definitions
        private static readonly List<RegisterUser> qaUserAccount = new List<RegisterUser>
        {
            { new RegisterUser{Email = UserQa1+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId0, OperatorId = operatorId0, Role = UserRole1} },
            { new RegisterUser{Email = UserQa1+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId1, OperatorId = operatorId1, Role = UserRole2} },
            { new RegisterUser{Email = UserQa1+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId2, OperatorId = operatorId2, Role = UserRole1} },
            { new RegisterUser{Email = UserQa2+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId0, OperatorId = operatorId0, Role = UserRole2} },
            { new RegisterUser{Email = UserQa2+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId1, OperatorId = operatorId1, Role = UserRole1} },
            { new RegisterUser{Email = UserQa2+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId2, OperatorId = operatorId2, Role = UserRole2} },
            { new RegisterUser{Email = UserQa3+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId0, OperatorId = operatorId0, Role = UserRole1} },
            { new RegisterUser{Email = UserQa3+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId1, OperatorId = operatorId1, Role = UserRole2} },
            { new RegisterUser{Email = UserQa3+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserQa1+operatorId2, OperatorId = operatorId2, Role = UserRole1} },
        };
        #endregion

        #region Seeder Methods
        public static async void SeedUserData(IDataAccess dataAccess, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, string environment)
        {
            if (environment.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
            {
                localUserAccount.AddRange(devUserAccount);
            }
            else if (environment.Equals("Staging", StringComparison.InvariantCultureIgnoreCase))
            {
                localUserAccount.AddRange(qaUserAccount);
            }

            foreach (var regUser in localUserAccount)
            {
                if (userManager.FindByNameAsync(regUser.Username).Result == null)
                {
                    var user = new AppUser
                    {
                        UserName = regUser.Username,
                        Email = regUser.Email,
                        DisplayUsername = regUser.Username,
                        RegistrationStatus = Enum.GetName(typeof(RegistrationStatusNames), RegistrationStatusNames.Registered),
                    };

                    IdentityResult idResult = await userManager.CreateAsync(user, regUser.Password);
                    if (idResult.Succeeded)
                    {
                        await dataAccess.CreateBackOfficeUserById(user.Id, regUser.OperatorId);
                        var roleCheck = await roleManager.RoleExistsAsync(regUser.Role);
                        if (!roleCheck)
                            await roleManager.CreateAsync(new IdentityRole(regUser.Role));
                        await userManager.AddToRoleAsync(user, regUser.Role);
                    }

                }

            }
        }
        #endregion



    }
}
