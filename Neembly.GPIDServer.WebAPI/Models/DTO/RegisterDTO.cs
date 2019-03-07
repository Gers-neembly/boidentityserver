using Neembly.BOIDServer.SharedClasses;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Neembly.BOIDServer.WebAPI.Models.DTO
{
    public class RegisterDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int OperatorId { get; set; }
        public string HostedUrl { get; set; }
        public string RoleType { get; set; }
        public BackOfficeUserInfo BackOfficeUserInfo { get; set; }
    }
}
