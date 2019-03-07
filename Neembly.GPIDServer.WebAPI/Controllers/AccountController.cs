using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedServices.Interfaces;
using Neembly.BOIDServer.WebAPI.Models.DTO;

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
            if (registerInfo.Password != registerInfo.ConfirmPassword)
            {
                return NotFound("Passwords do not matched.");
            }

            int clientOperatorId = registerInfo.OperatorId;

            string clientUsername = registerInfo.UserName + '_' + clientOperatorId.ToString();

            AppUser BackOfficeUser = _dataAccess.GetAppUser(registerInfo.Email, clientUsername, clientOperatorId);
            if (BackOfficeUser != null)
                return NotFound("User already exists.");

            var user = new AppUser
            {
                UserName = clientUsername,
                OperatorId = clientOperatorId,
                Email = registerInfo.Email,
                DisplayUsername = registerInfo.UserName,
                RegistrationStatus = Enum.GetName(typeof(RegistrationStatusNames), RegistrationStatusNames.Pending)
            };

            var result = await _userManager.CreateAsync(user, registerInfo.Password);
            if (!result.Succeeded)
                return NotFound("Registration failed.");

            string urlReferer = Request.Headers["Origin"].ToString();
            string theRole = string.IsNullOrEmpty(registerInfo.RoleType) ? "BackOfficeUser" : registerInfo.RoleType.ToLower();
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("username", user.DisplayUsername));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));

            if (theRole == "BackOfficeUser")
            {
                user.BackOfficeUserId = await _dataAccess.CreateBackOfficeUserById(user.Id, user.OperatorId, registerInfo.BackOfficeUserInfo);
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("operatorId", user.OperatorId.ToString()));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("backofficeuserId", user.BackOfficeUserId));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("registrationStatus", user.RegistrationStatus));
            }

            await CreateUserRoles(user, theRole);

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
                "verifyemail", "account",
                values: new { userId = user.Id, code = emailConfirmationToken, operatorId = user.OperatorId, urlreferer = urlReferer, urlhosted = registerInfo.HostedUrl },
                protocol: Request.Scheme);

            bool registerationCompleted = !string.IsNullOrEmpty(emailConfirmationToken);

            if (registerationCompleted)
            {
                await SendWelcomeEmail(urlReferer, user.DisplayUsername, user.Email);
                await SendActivationEmail(callbackUrl, user.DisplayUsername, user.Email);
            }

            return Ok();
        }
        #endregion

        #region Verify Email
        [Route("verifyemail")]
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId, string code, int operatorId, string urlreferer, string urlhosted)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, code);
                if (emailConfirmationResult.Succeeded)
                { 
                    if (!string.IsNullOrEmpty(user.BackOfficeUserId))
                        await SetRegistrationStatus(userId, RegistrationStatusNames.Registered);
                    return Redirect(urlreferer);
                }
            }
            return NotFound("Email verification code failed.");
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
        private async Task<bool> SetRegistrationStatus(string userId, RegistrationStatusNames registrationStatus)
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

        #endregion
    }
}