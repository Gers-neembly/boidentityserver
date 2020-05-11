using IdentityServer4;
using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedServices.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.SharedServices.Helpers
{
    public class TokenProviderService : ITokenProviderService
    {
        #region Member Variables
        private readonly IdentityServerTools _identityServerTools;
        private readonly AuthTokenInfo _authTokenInfo;
        #endregion

        #region Constructor
        public TokenProviderService(IdentityServerTools identityServerTools, AuthTokenInfo authTokenInfo)
        {
            _identityServerTools = identityServerTools;
            _authTokenInfo = authTokenInfo;
        }
        #endregion

        #region Actions

        #region Validate Token
        public async Task<bool> ValidateToken(string authToken)
        {
            return await Task.Run(() => VerifyToken(authToken));
        }
        #endregion

        #region Get Claims Permission
        public async Task<string> GetClaimsPermission(string authToken, string moduleName)
        {
            return await Task.Run(() => GetAccessPermission(authToken, moduleName));
        }
        #endregion

        #region Validate Permission
        public async Task<bool> HasValidPermission(string authToken, string moduleName, string claimValues)
        {
            return await Task.Run(() => IsValidPermission (authToken, moduleName, claimValues));
        }
        #endregion


        #region private methods
        private bool VerifyToken(string authToken)
        {
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            var tokenS = jwtHandler.ReadJwtToken(authToken);
            int validCounter = 0;
            if (DateTime.UtcNow > tokenS.ValidTo)
            {
                return false;
            }
            var claims = tokenS.Claims.Where(claim => claim.Type.Contains(GlobalConstants.TokenClaims.Client_Id, StringComparison.InvariantCultureIgnoreCase) || claim.Type.Contains(GlobalConstants.TokenClaims.Issuer, StringComparison.InvariantCultureIgnoreCase)).ToList();
            foreach (var claim in claims)
            {
                if (claim.Type.Equals(GlobalConstants.TokenClaims.Client_Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (claim.Value.Equals(_authTokenInfo.ClientId, StringComparison.InvariantCultureIgnoreCase))
                    { validCounter++; }
                    else
                    { return false; }
                }
                if (claim.Type.Equals(GlobalConstants.TokenClaims.Issuer, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (claim.Value.Equals(_authTokenInfo.ApiUrl, StringComparison.InvariantCultureIgnoreCase))
                    { validCounter++; }
                    else
                    { return false; }
                }
            }
            return (validCounter == 2);
        }

        private string GetAccessPermission(string authToken, string moduleName)
        {
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            var tokenS = jwtHandler.ReadJwtToken(authToken);
            string accessPermission = "";
            foreach (var claim in tokenS.Claims)
            {
                if (claim.Type.Equals(moduleName))
                {
                    accessPermission = claim.Value;
                    break;
                }
            }
            return accessPermission;
        }

        private bool IsValidPermission(string authToken, string moduleName, string claimValues)
        {
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            var tokenS = jwtHandler.ReadJwtToken(authToken);
            bool found = false;
            string[] accessPermission = claimValues.Split(",");
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

    #endregion
}
