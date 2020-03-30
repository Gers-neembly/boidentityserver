using Neembly.BOIDServer.SharedClasses.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.WebAPI.Models.DTO.Inputs
{
    public class ResetPasswordDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
