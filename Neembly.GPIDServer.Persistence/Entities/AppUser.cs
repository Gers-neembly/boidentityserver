using Microsoft.AspNetCore.Identity;
using System;

namespace Neembly.BOIDServer.Persistence.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayUsername { get; set; }
        public string RegistrationStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
