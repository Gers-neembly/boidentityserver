using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neembly.BOIDServer.Persistence.Entities
{
    public class OperatorData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OperatorId { get; set; }
        public long TagId { get; set; }


    }
}
