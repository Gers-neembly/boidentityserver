using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedClasses.Outputs;
using Neembly.BOIDServer.SharedServices.Interfaces;
using Neembly.BOIDServer.WebAPI.Models.DTO.Inputs;

namespace Neembly.BOIDServer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Member Variable
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IDataAccess _dataAccess;
        private readonly IEmailDispatcher _emailDispatcher;
        #endregion

        #region Constructor
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IDataAccess dataAccess,
            IEmailDispatcher emailDispatcher
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dataAccess = dataAccess;
            _emailDispatcher = emailDispatcher;
        }
        #endregion

        #region Actions

        #region Profiles
        [Route("profile")]
        [HttpPut]
        public async Task<IActionResult> Profile([FromBody] ProfileUpdateDTO profileUpdateInfo)
        {
            var dataInfo = await _dataAccess.ProfileRequestChange(profileUpdateInfo.BackOfficeUserId,
                                    new BackOfficeUserInfo
                                    {
                                        FirstName = profileUpdateInfo.BackOfficeUserInfo.FirstName,
                                        LastName = profileUpdateInfo.BackOfficeUserInfo.LastName,
                                        MobileNo = profileUpdateInfo.BackOfficeUserInfo.MobileNo,
                                        MobilePrefix = profileUpdateInfo.BackOfficeUserInfo.MobilePrefix
                                    });
            return Ok(dataInfo);
        }
        #endregion

        #region Register
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerInfo)
        {
            AppUser user = null;
            string userId = string.Empty;

            if (_dataAccess.UserOperatorExists(registerInfo.Email, registerInfo.UserName, registerInfo.OperatorId))
                return NotFound(GlobalConstants.ErrExistingAccount);
            else
            {
                registerInfo.Password = RandomGenerator.RandomPassword(8);
                registerInfo.ConfirmPassword = registerInfo.Password;
                registerInfo.BackOfficeUserInfo.InitialPassword = registerInfo.Password;

                user = new AppUser
                {
                    UserName = registerInfo.UserName,
                    Email = registerInfo.Email,
                    DisplayUsername = registerInfo.UserName,
                    RegistrationStatus = System.Enum.Parse(typeof(BOUserStatus), registerInfo.Status).ToString(),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, registerInfo.Password);
                if (!result.Succeeded)
                    return NotFound(GlobalConstants.ErrCreateAccount);

                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("username", user.DisplayUsername));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("registrationStatus", user.RegistrationStatus));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("operatorId", registerInfo.OperatorId.ToString()));

                if (registerInfo.Roles != null)
                {
                    foreach (var roleItem in registerInfo.Roles)
                        await CreateUserRoles(user, roleItem);
                }

                userId = user.Id;
                int backOfficeUserId = await _dataAccess.CreateBackOfficeUserById(userId, registerInfo.OperatorId, registerInfo.BackOfficeUserInfo);
                if (user != null)
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("backofficeId", backOfficeUserId.ToString()));

                return Ok();
            }
        }
        #endregion

        #region PrivateMethod UserRoles
        private async Task CreateUserRoles(AppUser user, string roleDesired)
        {
            IdentityResult roleResult;
            var roleCheck = await _roleManager.RoleExistsAsync(roleDesired);
            if (!roleCheck)
            {
                roleResult = await _roleManager.CreateAsync(new IdentityRole(roleDesired));
            }
            await _userManager.AddToRoleAsync(user, roleDesired);
        }
        #endregion

        #region PrivateMethod Registration Status
        private async Task<bool> SetRegistrationStatus(string userId, BOUserStatus registrationStatus)
        {
            return await _dataAccess.SetRegistrationStatus(userId, registrationStatus);
        }
        #endregion

        #region PrivateMethod Emails
        private async Task SendWelcomeEmail(string referer, string name, string email)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
            if (environmentName != "release"
                 || environmentName != "production")
            {
                await _emailDispatcher.SendWelcomeEmail(referer, name, email);
            }
        }

        private async Task SendActivationEmail(string content, string name, string email)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
            if (environmentName != "release"
                 || environmentName != "production")
            {
                await _emailDispatcher.SendActivationLink(content, name, email);
            }
        }
        #endregion

        #region Password
        [Route("reset-password")]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPasswordDTO user)
        {
            AppUser boUser = _dataAccess.GetAppUser(user.Email, user.Username);
            if (boUser == null)
                return NotFound(GlobalConstants.ErrUserAccountNotExisting);

            string token = await _userManager.GeneratePasswordResetTokenAsync(boUser);
            var result = await _userManager.ResetPasswordAsync(boUser, token, user.NewPassword);
            return Ok(result);
        }

        #endregion

        #region Claims or Permissions
        [Route("save-claims")]
        [HttpPost]
        public async Task<IActionResult> SaveUserClaims([FromBody] ClaimsDTO claimsInfo)
        {
            List<ClaimsViewModel> existClaims = null;
            AppUser boUser = _dataAccess.GetAppUser(claimsInfo.Email, claimsInfo.Username);
            if (boUser != null)
                existClaims = await _dataAccess.GetUserClaims(boUser.Id);
            else
                return NotFound(GlobalConstants.ErrUserAccountNotExisting);

            foreach (var item in claimsInfo.Permissions)
            {
                if (existClaims != null && existClaims.Any(a => a.ClaimType == item.ClaimType))
                {
                    var current = existClaims.Find(x => x.UserId == boUser.Id && x.ClaimType == item.ClaimType);
                    await _userManager.ReplaceClaimAsync(boUser
                        , new System.Security.Claims.Claim(current.ClaimType, System.Enum.Parse(typeof(ClaimValue), current.ClaimValue).ToString())
                        , new System.Security.Claims.Claim(item.ClaimType, System.Enum.Parse(typeof(ClaimValue), item.ClaimValue).ToString()));
                }
                else
                {
                    await _userManager.AddClaimAsync(boUser, new System.Security.Claims.Claim(item.ClaimType, System.Enum.Parse(typeof(ClaimValue), item.ClaimValue).ToString()));
                }
            }

            return Ok();
        }

        [HttpGet("claims/{email}/{username}")]
        public async Task<IActionResult> GetUserClaims(string email, string username)
        {
            List<ClaimsViewModel> claims = null;
            AppUser boUser = _dataAccess.GetAppUser(email, username);

            string userId = string.Empty;

            if (boUser != null)
            {
                claims = await _dataAccess.GetUserClaims(boUser.Id);
            }

            return Ok(claims);
        }


        #endregion

        #endregion
    }
}