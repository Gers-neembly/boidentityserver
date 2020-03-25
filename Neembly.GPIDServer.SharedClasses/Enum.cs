using System.ComponentModel;

namespace Neembly.BOIDServer.SharedClasses
{
    public enum RegistrationStatusNames
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Registered")]
        Registered = 2,
        [Description("Abandon")]
        Abandon = 3,
    }

    public enum HttpTransactType
    {
        [Description("Post")]
        Post = 1,
        [Description("Put")]
        Put = 2,
        [Description("Get")]
        Get = 3,
    }

    public enum BOUserStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2
    }

    public enum ClaimValue
    {
        [Description("NotPermitted")]
        NotPermitted = 1,
        [Description("Permitted")]
        Permitted = 2,
        [Description("CanModify")]
        CanModify = 3
    }
}
