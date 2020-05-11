using IdentityServer4;
using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedServices.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
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
            foreach (var claim in tokenS.Claims)
            {
                if (claim.Type.Equals("client_id"))
                {
                    if (claim.Value.Equals(_authTokenInfo.ClientId))
                    { validCounter++; }
                    else
                    { return false; }
                }
                if (claim.Type.Equals("iss"))
                {
                    if (claim.Value.Equals(_authTokenInfo.ApiUrl))
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
        #endregion
    }
        
    #endregion
    }
