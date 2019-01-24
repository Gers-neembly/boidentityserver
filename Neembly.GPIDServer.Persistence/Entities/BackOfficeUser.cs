using Neembly.GPIDServer.SharedClasses;
using System.ComponentModel.DataAnnotations;

namespace Neembly.GPIDServer.Persistence.Entities
{
    public class BackOfficeUser
    {
        [Key]
        public string BackOfficeUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePrefix { get; set; }
        public string MobileNo { get; set; }
    }
}
