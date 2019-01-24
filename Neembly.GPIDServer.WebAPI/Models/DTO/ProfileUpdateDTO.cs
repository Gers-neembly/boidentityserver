using Neembly.GPIDServer.SharedClasses;

namespace Neembly.GPIDServer.WebAPI.Models.DTO
{
    public class ProfileUpdateDTO
    {
        public string BackOfficeUserId { get; set; } 
        public BackOfficeUserInfo BackOfficeUserInfo { get; set; }
    }
}
