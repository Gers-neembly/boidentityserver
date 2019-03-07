using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.WebAPI.Models.Configs
{
    public class AuthClientResources
    {
        public string Name { get; set; }
        public string Request { get; set; }
        public string SecretKey { get; set; }
    }
}
