using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.WebAPI.Models.Configs
{
    public class AuthClientConfiguration
    {
        public List<AuthClientInfo> AuthClientInfoList { get; set; }
        public List<AuthClientResources> AuthClientResourcesList { get; set; }
    }
}
