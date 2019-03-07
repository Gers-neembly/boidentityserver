using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.WebAPI.Models.Configs
{
    public class AuthClientInfo
    {
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public string ApiScope { get; set; }
        public int LifeTime { get; set; }
    }
}
