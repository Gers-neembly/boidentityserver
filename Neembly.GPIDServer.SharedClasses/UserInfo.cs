using System;

namespace Neembly.BOIDServer.SharedClasses
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
    }
}
