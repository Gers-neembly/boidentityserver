using Neembly.BOIDServer.SharedClasses;

namespace Neembly.BOIDServer.WebAPI.Models.DTO
{
    public class ProfileUpdateDTO
    {
        public string BackOfficeUserId { get; set; } 
        public BackOfficeUserInfo BackOfficeUserInfo { get; set; }
    }
}
