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
        private const string UserDev1 = "jc";
        private const string UserDev2 = "mel";
        private const string UserTag1 = "bouserA";
        private const string UserTag2 = "bouserB";
        private const string UserRole1 = "testerA";
        private const string UserRole2 = "testerB";
        private const string UserEmailTag = "@gmail.com";
        private const string UserDefaultPassword = "password";
        #endregion

        #region Data Definitions

        private static readonly IList<RegisterUser> registerUserAccount = new List<RegisterUser>
        {
            { new RegisterUser{Email = UserDev1+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserDev1+operatorId1, OperatorId = operatorId1, Role = UserRole1} },
            { new RegisterUser{Email = UserDev2+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserDev2+operatorId2, OperatorId = operatorId2, Role = UserRole2} },
            { new RegisterUser{Email = UserTag1+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserTag1+operatorId0, OperatorId = operatorId0, Role = UserRole1} },
            { new RegisterUser{Email = UserTag1+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserTag1+operatorId1, OperatorId = operatorId1, Role = UserRole1} },
            { new RegisterUser{Email = UserTag1+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserTag1+operatorId2, OperatorId = operatorId2, Role = UserRole1} },
            { new RegisterUser{Email = UserTag2+operatorId0+UserEmailTag, Password = UserDefaultPassword, Username = UserTag2+operatorId0, OperatorId = operatorId0, Role = UserRole2} },
            { new RegisterUser{Email = UserTag2+operatorId1+UserEmailTag, Password = UserDefaultPassword, Username = UserTag2+operatorId1, OperatorId = operatorId1, Role = UserRole2} },
            { new RegisterUser{Email = UserTag2+operatorId2+UserEmailTag, Password = UserDefaultPassword, Username = UserTag2+operatorId2, OperatorId = operatorId2, Role = UserRole2} },
        };

        #endregion

        #region Seeder Methods
        public static async void SeedUserData(IDataAccess dataAccess, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var regUser in registerUserAccount)
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
