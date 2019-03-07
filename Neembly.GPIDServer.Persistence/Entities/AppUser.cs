using Microsoft.AspNetCore.Identity;

namespace Neembly.BOIDServer.Persistence.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayUsername { get; set; }
        public int OperatorId { get; set; }
        public string BackOfficeUserId { get; set; }
        public string RegistrationStatus { get; set; }
    }
}
