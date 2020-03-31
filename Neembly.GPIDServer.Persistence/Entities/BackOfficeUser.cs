using System.ComponentModel.DataAnnotations;

namespace Neembly.BOIDServer.Persistence.Entities
{
    public class BackOfficeUser
    {
        [Key]
        public string NetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePrefix { get; set; }
        public string MobileNo { get; set; }
        public string InitialPassword { get; set; }
        public bool IsPasswordReset { get; set; }
    }
}
