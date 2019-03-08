using Neembly.BOIDServer.SharedClasses;
using System.Collections.Generic;

namespace Neembly.BOIDServer.WebAPI.Models.DTO.Inputs
{
    public class RegisterDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int OperatorId { get; set; }
        public List<string> Roles { get; set; }
        public BackOfficeUserInfo BackOfficeUserInfo { get; set; }
    }
}
