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
        private const string UserTag1 = "bouser1";
        private const string UserTag2 = "bouser2";
        private const string UserRole1 = "tester1";
        private const string UserRole2 = "tester2";
        #endregion

        #region Data Definitions

        private static readonly IList<RegisterUser> registerUserAccount = new List<RegisterUser>
        {
            { new RegisterUser{Email = UserTag1+operatorId0+"@gmail.com", Password = "password", Username = UserTag1+operatorId0, OperatorId = operatorId0, Role = UserRole1} },
            { new RegisterUser{Email = UserTag1+operatorId1+"@gmail.com", Password = "password", Username = UserTag1+operatorId1, OperatorId = operatorId1, Role = UserRole1} },
            { new RegisterUser{Email = UserTag1+operatorId2+"@gmail.com", Password = "password", Username = UserTag1+operatorId2, OperatorId = operatorId2, Role = UserRole1} },
            { new RegisterUser{Email = UserTag2+operatorId0+"@gmail.com", Password = "password", Username = UserTag2+operatorId0, OperatorId = operatorId0, Role = UserRole2} },
            { new RegisterUser{Email = UserTag2+operatorId1+"@gmail.com", Password = "password", Username = UserTag2+operatorId1, OperatorId = operatorId1, Role = UserRole2} },
            { new RegisterUser{Email = UserTag2+operatorId2+"@gmail.com", Password = "password", Username = UserTag2+operatorId2, OperatorId = operatorId2, Role = UserRole2} },
        };

        #endregion

        #region Seeder Methods
        public static async void SeedUserData(AppDBContext context, IDataAccess dataAccess)
        {
            foreach(var regUser in registerUserAccount)
            {
                var user = new AppUser
                {
                    UserName = regUser.Username,
                    Email = regUser.Email,
                    NormalizedUserName = regUser.Username,
                    NormalizedEmail = regUser.Email,
                    EmailConfirmed = false,
                    LockoutEnabled = false,
                    DisplayUsername = regUser.Username,
                    RegistrationStatus = Enum.GetName(typeof(RegistrationStatusNames), RegistrationStatusNames.Registered),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == regUser.Role))
                {
                    await roleStore.CreateAsync(new IdentityRole { Name = regUser.Role, NormalizedName = regUser.Role });
                }

                if (!context.Users.Any(r => r.UserName == regUser.Username))
                {
                    var password = new PasswordHasher<AppUser>();
                    var hashed = password.HashPassword(user, regUser.Password);
                    user.PasswordHash = hashed;
                    var userStore = new UserStore<AppUser>(context);
                    await userStore.CreateAsync(user);
                    await userStore.AddToRoleAsync(user, regUser.Role);
                    await dataAccess.CreateBackOfficeUserById(user.Id, regUser.OperatorId);
                }
            }
            await context.SaveChangesAsync();
        }
        #endregion



    }
}
