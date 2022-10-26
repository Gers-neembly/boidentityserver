using System;
using System.Collections.Generic;
using System.Text;

namespace Neembly.BOIDServer.SharedClasses
{
    public class AuthTokenInfo
    {
        public string ClientId { get; set; }
        public int LifeTime { get; set; }
        public string ApiUrl { get; set; }
        public bool SecuredHttps { get; set; }
    }
}
