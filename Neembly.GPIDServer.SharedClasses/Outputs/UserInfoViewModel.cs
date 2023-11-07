using Neembly.BOIDServer.SharedClasses.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neembly.BOIDServer.SharedClasses.Outputs
{
    public class UserInfoViewModel
    {
        public PagedResult<UserInfo> UserInfos { get; set; }
    }
}
