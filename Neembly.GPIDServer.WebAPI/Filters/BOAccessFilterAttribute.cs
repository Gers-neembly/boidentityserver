using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.SharedClasses;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Neembly.BOIDServer.WebAPI.Filters
{
    public class BOAccessFilter : ActionFilterAttribute //, IAuthorizationFilter
    {
        #region Member Variable
        public string _argument { get; }
        public string _values { get; }
        #endregion

        #region Constructor
        public BOAccessFilter(string argument, string values)
        {
            _argument = argument;
            _values = values;
        }
        #endregion  

        #region OnAuthorization Trigger
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string auth = filterContext.HttpContext.Request.Headers["Authorization"].ToString();
            var authTokenInfo = filterContext.HttpContext.RequestServices.GetRequiredService<AuthTokenInfo>();
            bool isPermitted = false;
            bool found = auth.IndexOf(GlobalConstants.TokenClaims.Bearer) == -1 ? false : true;
            if (string.IsNullOrEmpty(auth) || !found) filterContext.Result = new UnauthorizedResult();
            if (auth.Length > 15)
            {
                string accessToken = auth.Substring(7);
                if(ValidToken(accessToken, authTokenInfo)) isPermitted = HasValidPermission(accessToken, _argument, _values);
                if (!isPermitted) filterContext.Result = new UnauthorizedResult();
            }
        }
        #endregion

        #region private methods
        private bool ValidToken(string authToken, AuthTokenInfo tokenInfo)
        {
            int validCounter = 0;
            try
            {
                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
                var tokenS = jwtHandler.ReadJwtToken(authToken);
                if (DateTime.UtcNow > tokenS.ValidTo)
                {
                    return false;
                }
                var claims = tokenS.Claims.Where(claim => claim.Type.Contains(GlobalConstants.TokenClaims.Client_Id, StringComparison.InvariantCultureIgnoreCase) || claim.Type.Contains(GlobalConstants.TokenClaims.Issuer, StringComparison.InvariantCultureIgnoreCase)).ToList();
                foreach (var claim in claims)
                {
                    if (claim.Type.Equals(GlobalConstants.TokenClaims.Client_Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (claim.Value.Equals(tokenInfo.ClientId, StringComparison.InvariantCultureIgnoreCase))
                        { validCounter++; }
                        else
                        { return false; }
                    }
                    if (claim.Type.Equals(GlobalConstants.TokenClaims.Issuer, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!tokenInfo.SecuredHttps)
                        {
                            if (((new Uri(claim.Value)).Host).Equals((new Uri(tokenInfo.ApiUrl)).Host, StringComparison.InvariantCultureIgnoreCase)) { validCounter++; }
                            else { return false; }
                        }
                        else
                        {
                            if (claim.Value.Equals(tokenInfo.ApiUrl, StringComparison.InvariantCultureIgnoreCase))
                            { validCounter++; }
                            else
                            { return false; }
                        }
                    }
                }
            }
            catch (Exception) { return false; }
            return (validCounter == 2);
        }

        private bool HasValidPermission(string authToken, string moduleName, string claimValues)
        {
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            var tokenS = jwtHandler.ReadJwtToken(authToken);
            bool found = false;
            string[] values = claimValues.Split("=");
            string[] accessPermission = values[1].Split(",");
            var claims = tokenS.Claims.Where(claim => moduleName.Contains(claim.Type)).ToList();
            foreach (var claimItem in claims)
            {
                foreach (var accessLevel in accessPermission)
                {
                    if (accessLevel.Equals(claimItem.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }
        #endregion

    }
}
