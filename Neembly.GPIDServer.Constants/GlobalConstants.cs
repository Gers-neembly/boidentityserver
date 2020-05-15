namespace Neembly.BOIDServer.Constants
{
    public static class GlobalConstants
    {
        #region Message Codes
        public const string ErrRegUsername = "REGV-001";
        public const string ErrPasswordValue = "REGV-002";
        public const string ErrPasswordLength = "REGV-003";
        public const string ErrConfirmPasswordValue = "REGV-004";
        public const string ErrConfirmPasswordLength = "REGV-006";
        public const string ErrPasswordsMismatch = "REGV-008";
        public const string ErrEmailValue = "REGV-010";
        public const string ErrEmailFormat = "REGV-011";
        public const string ErrCreateAccount = "REGD-001";
        public const string ErrExistingAccount = "REGD-002";
        public const string ErrUsernameAccountNotRegistered = "LOGD-001";
        public const string ErrUserAccountNotExisting = "LOGD-003";
        #endregion

        #region Auth
        public const string ApiScope = "Neembly.GP.Web.PlayerPortalApi";
        #endregion

        #region Operator Tag Ids
        public const int PlayerIdTagStarts = 10000;
        #endregion

        #region Defined Modules
        public static class Modules
        {
            public const string UserManagement = "USERMANAGEMENT";
        }
        #endregion

        #region Permission Values
        public static class AccessPermission
        {
            public const string Permitted = "Permitted";
            public const string CanModify = "CanModify";
            public const string AllowAccessToUAC = "USERMANAGEMENT=CanModify,Permitted";
        }
        #endregion

        #region Authentication Claims
        public static class TokenClaims
        {
            public const string Client_Id = "client_id";
            public const string Issuer = "iss";
            public const string Bearer = "Bearer";
        }
        #endregion

    }
}
