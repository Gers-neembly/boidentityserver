using Neembly.BOIDServer.SharedClasses;

namespace Neembly.BOIDServer.WebAPI.Models.DTO.Inputs
{
    public class ProfileUpdateDTO
    {
        public string BackOfficeUserId { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public BackOfficeUserInfo BackOfficeUserInfo { get; set; }
    }
}
