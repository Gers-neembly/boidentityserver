using System.ComponentModel.DataAnnotations;

namespace Neembly.BOIDServer.WebAPI.Models.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int OperatorId { get; set; }
    }
}
