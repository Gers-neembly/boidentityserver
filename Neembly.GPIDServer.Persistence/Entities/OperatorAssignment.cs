using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neembly.BOIDServer.Persistence.Entities
{
    public class OperatorAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string NetUserId { get; set; }
        public int OperatorId { get; set; }
        public string BackOfficeUserId { get; set; }
    }
}
